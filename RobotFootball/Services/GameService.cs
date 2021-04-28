using Microsoft.AspNetCore.SignalR;
using RobotFootball.Hubs;
using RobotFootball.RobotFootballGame;
using RobotFootball.RobotFootballGame.Interfaces;
using System.Threading.Tasks;

namespace RobotFootball.Services
{
    public class GameService : IGameService
    {
        private readonly IHubContext<RobotFootballHub, IGameClient> _hubContext;

        public GameService(IHubContext<RobotFootballHub, IGameClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task<Game> CreateGame()
        {
            return new Game(_hubContext);
        }
    }
}
