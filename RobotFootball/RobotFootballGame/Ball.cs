using RobotFootball.RobotFootballGame.Extensions;
using RobotFootball.RobotFootballGame.Helper;
using System;

namespace RobotFootball.RobotFootballGame
{
    public class Ball : GameObject
    {
        private static readonly double DRAG = 0.2;
        private static readonly double SHOT_SPEED = 40.0;
        private static readonly double COLLISION_SPEED_ABSORPTION = 20.0;

        public double Speed { get; set; }
        public Vector Direction { get; set; }

        public void Move(double elapsedTime)
        {
            var movement = Direction.MultNew(Speed * elapsedTime);

            Position.Plus(movement);
            Speed -= movement.Magnitude() * DRAG;
        }

        public void ModifyMovement(Vector normal, bool isShot)
        {
            ModifyDirection(normal);
            ModifySpeed(isShot);
        }

        private void ModifyDirection(Vector normal)
        {
            if (Direction.IsNullVector())
            {
                Direction.Set(normal.MultNew(-1));
            }

            var ballAngle =  Direction.Angle.EnsurePositiveAngle();
            var normalAngle = normal.Angle.EnsurePositiveAngle();
            var angleFromWall = (normalAngle - ballAngle - Math.PI / 2).EnsurePositiveAngle();
            var remainingFrom180 = (Math.PI - angleFromWall).EnsurePositiveAngle();
            var smallerAngleFromWall = angleFromWall < remainingFrom180 ? angleFromWall : remainingFrom180;
            var rotation = Math.PI - (smallerAngleFromWall * 2);

            if (angleFromWall == smallerAngleFromWall)
            {
                Direction.Rotate(-rotation);
            }
            else
            {
                Direction.Rotate(rotation);
            }

            Direction.Mult(-1);
        }

        private void ModifySpeed(bool isShot)
        {
            if (isShot)
            {
                Speed = SHOT_SPEED;
            }
            else
            {
                Speed = Speed * (100 - COLLISION_SPEED_ABSORPTION) / 100;
            }
        }
    }
}
