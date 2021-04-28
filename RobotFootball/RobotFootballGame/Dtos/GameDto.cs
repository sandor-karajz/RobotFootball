using RobotFootball.RobotFootballGame.Enums;
using System.Collections.Generic;

namespace RobotFootball.RobotFootballGame.Dtos
{
    public class GameDto
    {
        public int PlayerIndex { get; set; }
        public GameStatus Status { get; set; }
        public List<int> PlayerScores { get; set; }
        public GameObjectDto Ball { get; set; }
        public List<PlayerDto> Players { get; set; }
    }
}
