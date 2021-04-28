using System;

namespace RobotFootball.RobotFootballGame.Extensions
{
    public static class DoubleExtensions
    {
        public static double EnsurePositiveAngle(this double angle)
        {
            return angle < 0 ? angle + Math.PI * 2 : angle;
        }
    }
}
