namespace RobotFootball.RobotFootballGame
{
    public class Command
    {
        public int PlayerId { get; set; }
        public double TargetX { get; set; }
        public double TargetY { get; set; }
        public bool IsShooting { get; set; }
    }
}
