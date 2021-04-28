using Microsoft.AspNetCore.SignalR;
using RobotFootball.Hubs;
using RobotFootball.RobotFootballGame.Dtos;
using RobotFootball.RobotFootballGame.Enums;
using RobotFootball.RobotFootballGame.Helper;
using RobotFootball.RobotFootballGame.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace RobotFootball.RobotFootballGame
{
    public class Game
    {
        private static readonly double GOAL_LINE_LENGHT = 75.0;
        private static readonly double TOUCHLINE_LENGHT = 110.0;
        private static readonly double GOAL_LENGHT = 7.32;
        private static readonly int FPS = 60;

        private readonly IHubContext<RobotFootballHub, IGameClient> _hubContext;

        private Timer _timer;
        private DateTime _startTime;
        private DateTime _previousTime;

        private readonly string _id;
        private  GameStatus _status;
        private readonly Ball _ball;
        private readonly List<Border> _borders;
        private readonly List<Player> _players = new();
        private readonly List<Goal> _goals = new();
        private readonly List<FootballPlayer> _footballPlayers = new();

        public Game(IHubContext<RobotFootballHub, IGameClient> hubContext)
        {
            _hubContext = hubContext;

            _id = Guid.NewGuid().ToString();
            _status = GameStatus.WaitingForPlayer;
            _ball = GetInitialBall();
            _borders = GetBorders();

            Enumerable.Range(0, 2).ToList().ForEach(x =>
            {
                var player = new Player();

                _players.Add(player);
                _goals.Add(GetGoal(x));

                Enumerable.Range(0, 11).ToList().ForEach(y =>
                {
                    _footballPlayers.Add(GetInitialFootballPlayer(x, y));
                });
            });
        }

        public string Id => _id;
        public GameStatus Status => _status;

        public async Task Start()
        {
            _status = GameStatus.InProgress;

            SetTimer();
            await SendState();
        }

        public async Task Finish()
        {
            _status = GameStatus.Finished;

            if (_timer is not null)
            {
                _timer.Enabled = false;
            }

            await SendState();
        }

        public void CommandPlayers(List<Command> commands,  string connectionId)
        {
            commands.ForEach(x =>
            {
                if (x.PlayerId >= 0 && x.PlayerId < 11 && x.TargetX >= 0 && x.TargetX <= TOUCHLINE_LENGHT && x.TargetY >= 0 && x.TargetY <= GOAL_LINE_LENGHT)
                {
                    var team = _players.FindIndex(y => y.ConnectionId == connectionId);
                    var player = _footballPlayers.First(y => y.TeamId == team && y.PlayerId == x.PlayerId);

                    if (player.TargetDestination is null)
                    {
                        player.TargetDestination = new();
                    }

                    player.TargetDestination.X = x.TargetX;
                    player.TargetDestination.Y = x.TargetY;
                    player.IsShooting = x.IsShooting;
                }
            });
        }

        public void Score(int teamId)
        {
            _players[teamId].IncreaseScore();

            _ball.Position = GetInitialBallPosition();
            _ball.Direction.Set(0, 0);
            _ball.Speed = 0;

            _footballPlayers.ForEach(x =>
            {
                x.Position = GetInitialPlayerPosition(x.TeamId, x.PlayerId);
                x.TargetDestination = null;
                x.IsShooting = false;
            });
        }

        public void Rebound(Vector normal)
        {
            _ball.ModifyMovement(normal, false);
        }

        public void Shoot(Vector playerPosition, Vector playerPreviousPosition, bool isShooting)
        {
            var normal = _ball.Position.MinusNew(playerPosition).Normalize();

            if (normal.IsNullVector())
            {
                normal = playerPosition.MinusNew(playerPreviousPosition).Normalize();
            }

            _ball.ModifyMovement(normal, isShooting);
        }

        public async Task<bool> AddConnectionId(string connectionId)
        {
            var readyToStart = true;

            if (string.IsNullOrWhiteSpace(_players[0].ConnectionId))
            {
                readyToStart = false;

                _players[0].ConnectionId = connectionId;

                await SendState();
            }
            else if(string.IsNullOrWhiteSpace(_players[1].ConnectionId))
            {
                _players[1].ConnectionId = connectionId;
            }

            return readyToStart;
        }

        public bool HasPlayerConnectionId(string connectionId) => _players.Any(x => x.ConnectionId == connectionId);

        private static Ball GetInitialBall() => new()
        {
            Position = GetInitialBallPosition(),
            Direction = new(),
            Radius = 0.22
        };

        private static List<Border> GetBorders()
        {
            List<Border> a = new()
            {
                new()
                {
                    Start = new() { X = 0, Y = 0 },
                    End = new() { X = TOUCHLINE_LENGHT, Y = 0 },
                    Normal = new() { X = 0, Y = 1 }
                },
                new()
                {
                    Start = new() { X = TOUCHLINE_LENGHT, Y = 0 },
                    End = new() { X = TOUCHLINE_LENGHT, Y = GetGoalYBottom() },
                    Normal = new() { X = -1, Y = 0 }
                },
                new()
                {
                    Start = new() { X = TOUCHLINE_LENGHT, Y = GetGoalYTop() },
                    End = new() { X = TOUCHLINE_LENGHT, Y = GOAL_LINE_LENGHT },
                    Normal = new() { X = -1, Y = 0 }
                },
                new()
                {
                    Start = new() { X = TOUCHLINE_LENGHT, Y = GOAL_LINE_LENGHT },
                    End = new() { X = 0, Y = GOAL_LINE_LENGHT },
                    Normal = new() { X = 0, Y = -1 }
                },
                new()
                {
                    Start = new() { X = 0, Y = GOAL_LINE_LENGHT },
                    End = new() { X = 0, Y = GetGoalYTop() },
                    Normal = new() { X = 1, Y = 0 }
                },
                new()
                {
                    Start = new() { X = 0, Y = GetGoalYBottom() },
                    End = new() { X = 0, Y = 0 },
                    Normal = new() { X = 1, Y = 0 }
                }
            };

            return a;
        }

        private static Goal GetGoal(int team) => new()
        {
            Start = new()
            {
                X = team == 0 ? TOUCHLINE_LENGHT : 0,
                Y = team == 0 ? GetGoalYBottom() : GetGoalYTop()
            },
            End = new()
            {
                X = team == 0 ? TOUCHLINE_LENGHT : 0,
                Y = team == 0 ? GetGoalYTop() : GetGoalYBottom()
            },
            Normal = new()
            {
                X = team == 0 ? -1 : 1,
                Y = 0
            },
            TeamId = team
        };

        private static FootballPlayer GetInitialFootballPlayer(int team, int number) => new()
        {
            PlayerId = number,
            TeamId = team,
            Radius = 0.5,
            Position = GetInitialPlayerPosition(team, number)
        };

        private static double GetGoalYBottom() => GOAL_LINE_LENGHT / 2 - GOAL_LENGHT / 2;

        private static double GetGoalYTop() => GOAL_LINE_LENGHT / 2 + GOAL_LENGHT / 2;

        private static Vector GetInitialBallPosition() {
            var random = new Random();

            return new()
            {
                X = TOUCHLINE_LENGHT / 2 + GetNoise(),
                Y = GOAL_LINE_LENGHT / 2 + GetNoise()
            };

            double GetNoise() => random.NextDouble() / 10 - 0.05;
        }

        private static Vector GetInitialPlayerPosition(int team, int number)
        {
            return new()
            {
                X = number switch
                {
                    0 => GetPositionX(1),
                    >= 0 and <= 4 => GetPositionX(2),
                    >= 5 and <= 7 => GetPositionX(3),
                    >= 8 and <= 10 => GetPositionX(4),
                    _ => 0
                },
                Y = number switch
                {
                    4 => GetPositionY(1, 5),
                    7 or 10 => GetPositionY(1, 4),
                    3 => GetPositionY(2, 5),
                    0 or 6 or 9 => GetPositionY(1, 2),
                    2 => GetPositionY(3, 5),
                    5 or 8 => GetPositionY(3, 4),
                    1 => GetPositionY(4, 5),
                    _ => 0
                }
            };

            double GetPositionX(int dividend)
            {
                var positionX = TOUCHLINE_LENGHT * dividend / 10;

                return team == 0 ? positionX : TOUCHLINE_LENGHT - positionX;
            }

            double GetPositionY(int dividend, int divisor) => GOAL_LINE_LENGHT * dividend / divisor;
        }

        private async Task SendState()
        {
            foreach (var i in Enumerable.Range(0, _players.Count))
            {
                if (!string.IsNullOrWhiteSpace(_players[i].ConnectionId))
                {
                    var game = new GameDto()
                    {
                        PlayerIndex = i,
                        Status = Status,
                        PlayerScores = _players.Select(x => x.Score).ToList(),
                        Ball = new()
                        {
                            Position = new() { X = _ball.Position.X, Y = _ball.Position.Y },
                            Radius = _ball.Radius
                        },
                        Players = _footballPlayers.Select(x => new PlayerDto()
                        {
                            PlayerId = x.PlayerId,
                            Position = new() { X = x.Position.X, Y = x.Position.Y },
                            Radius = x.Radius,
                            IsOpponent = x.TeamId != i
                        }).ToList()
                    };

                    await _hubContext.Clients.Client(_players[i].ConnectionId).SendState(game);
                }
            }
        }

        private void SetTimer()
        {
            _timer = new Timer(1000.0/FPS);
            _timer.Elapsed += OnTimeElapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _startTime = DateTime.Now;
            _previousTime = DateTime.Now;
        }

        private async void OnTimeElapsed(object source, ElapsedEventArgs elapsedEvent)
        {
            var elapsedTimeFromStart = elapsedEvent.SignalTime - _startTime;

            if (elapsedTimeFromStart.TotalMinutes > 2)
            {
                await Finish();
            }
            else
            {
                var elapsedTime = elapsedEvent.SignalTime - _previousTime;

                _previousTime = elapsedEvent.SignalTime;

                Simulate(elapsedTime.TotalSeconds);
                await SendState();
            }
        }

        private void Simulate(double elapsedTime)
        {
            SimulatePlayers(elapsedTime);
            SimulateBall(elapsedTime);
        }

        private void SimulatePlayers(double elapsedTime)
        {
            _footballPlayers.ForEach(x => x.Move(elapsedTime));
        }

        private void SimulateBall(double elapsedTime)
        {
            HandleCollisions();
            _ball.Move(elapsedTime);
        }

        private void HandleCollisions()
        {
            var collidingGoal = _goals.FirstOrDefault(x => x.IsCollide(_ball));

            if (collidingGoal is not null)
            {
                collidingGoal.ResolveCollision(this);
            }
            else
            {
                _borders.Where(x => x.IsCollide(_ball)).ToList().ForEach(x => x.ResolveCollision(this));

                _footballPlayers.Where(x => x.IsCollide(_ball)).OrderBy(x => x.Position.Distance(_ball.Position)).FirstOrDefault()?.ResolveCollision(this);
            }
        }
    }
}
