namespace RobotFootball.RobotFootballGame
{
    public class Goal : Border
    {
        public int TeamId { get; set; }

        public override void ResolveCollision(Game game)
        {
            game.Score(TeamId);
        }
    }
}
