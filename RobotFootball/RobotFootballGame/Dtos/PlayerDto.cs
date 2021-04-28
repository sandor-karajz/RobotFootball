namespace RobotFootball.RobotFootballGame.Dtos
{
    public class PlayerDto : GameObjectDto
    {
        public int PlayerId { get; set; }
        public bool IsOpponent { get; set; }
    }
}
