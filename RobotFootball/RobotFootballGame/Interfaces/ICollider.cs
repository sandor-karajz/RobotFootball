namespace RobotFootball.RobotFootballGame.Interfaces
{
    public interface ICollider
    {
        bool IsCollide(Ball ball);
        void ResolveCollision(Game game);
    }
}
