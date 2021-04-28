using RobotFootball.RobotFootballGame.Dtos;
using System.Threading.Tasks;

namespace RobotFootball.RobotFootballGame.Interfaces
{
    public interface IGameClient
    {
        Task Disconnect();
        Task SendState(GameDto game);
    }
}
