using Microsoft.AspNetCore.SignalR;
using RobotFootball.RobotFootballGame;
using RobotFootball.RobotFootballGame.Enums;
using RobotFootball.RobotFootballGame.Interfaces;
using RobotFootball.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RobotFootball.Hubs
{
    public class RobotFootballHub : Hub<IGameClient>
    {
        private static readonly List<Game> _games = new();
        private readonly IGameService _gameService;

        public RobotFootballHub(IGameService gameService)
        {
            _gameService = gameService;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            var game = _games.FirstOrDefault(x => x.Status == GameStatus.WaitingForPlayer);
            
            if (game is null)
            {
                game = await _gameService.CreateGame();

                _games.Add(game);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);

            var readyToStart = await game.AddConnectionId(Context.ConnectionId);

            if (readyToStart)
            {
                await game.Start();
            }
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var game = GetUsersGame();

            if (game is not null)
            {
                await game.Finish();

                await Clients.Group(game.Id).Disconnect();
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, game.Id);

                _games.Remove(game);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task Command(List<Command> commands)
        {
            var game = GetUsersGame();

            if (game is not null)
            {
                game.CommandPlayers(commands, Context.ConnectionId);
            }
        }

        private Game GetUsersGame()
        {
            return _games.FirstOrDefault(x => x.HasPlayerConnectionId(Context.ConnectionId));
        }
    }
}
