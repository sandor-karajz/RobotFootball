using RobotFootball.RobotFootballGame.Extensions;
using RobotFootball.RobotFootballGame.Helper;
using RobotFootball.RobotFootballGame.Interfaces;
using System;

namespace RobotFootball.RobotFootballGame
{
    public class Border : ICollider
    {
        public Vector Start { get; set; }
        public Vector End { get; set; }
        public Vector Normal { get; set; }

        public bool IsCollide(Ball ball)
        {
            var behindTheFront = ball.Position.Dot(Normal) < Start.Dot(Normal);
            var rightFromStart = ball.Position.Cross(Normal) > Start.Cross(Normal);
            var leftFromEnd = ball.Position.Cross(Normal) < End.Cross(Normal);
            var hasOppositeDirection = Math.Abs(ball.Direction.Angle.EnsurePositiveAngle() - Normal.Angle.EnsurePositiveAngle()) > Math.PI / 2;

            return behindTheFront && rightFromStart && leftFromEnd && hasOppositeDirection;
        }

        public virtual void ResolveCollision(Game game)
        {
            game.Rebound(Normal);
        }
    }
}
