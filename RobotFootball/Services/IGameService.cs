using RobotFootball.RobotFootballGame;
using System.Threading.Tasks;

namespace RobotFootball.Services
{
    public interface IGameService
    {
        Task<Game> CreateGame();
    }
}
