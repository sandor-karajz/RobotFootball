using RobotFootball.RobotFootballGame.Helper;
using RobotFootball.RobotFootballGame.Interfaces;

namespace RobotFootball.RobotFootballGame
{
    public class FootballPlayer : GameObject, ICollider
    {
        private static readonly double SPEED = 10.0;

        public int PlayerId { get; set; }
        public int TeamId { get; set; }
        public Vector PreviousPosition { get; set; }
        public Vector TargetDestination { get; set; }
        public bool IsShooting { get; set; }

        public void Move(double elapsedTime)
        {
            SetPreviousPosition();

            if (TargetDestination is not null)
            {
                var distanceCanBeMade = SPEED * elapsedTime;
                var playerTargetDistance = Position.Distance(TargetDestination);

                if (playerTargetDistance <= distanceCanBeMade)
                {
                    Position = TargetDestination;
                    TargetDestination = null;
                }
                else
                {
                    var direction = TargetDestination.MinusNew(Position).Normalize();
                    var movement = direction.MultNew(distanceCanBeMade);

                    Position.Plus(movement);
                }
            }
        }

        public bool IsCollide(Ball ball) => Position.Distance(ball.Position) < Radius + ball.Radius;

        public void ResolveCollision(Game game)
        {
            game.Shoot(Position, PreviousPosition, IsShooting);
        }

        private void SetPreviousPosition()
        {
            if (PreviousPosition is null)
            {
                PreviousPosition = new Vector();
            }

            PreviousPosition.Set(Position);
        }
    }
}
