using System;

namespace RobotFootball.RobotFootballGame.Helper
{
    public class Vector
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Angle => Math.Atan2(Y, X);

        public bool IsNullVector()
        {
            return X == 0 && Y == 0;
        }

        public Vector Clone()
        {
            return new Vector() { X = X, Y = Y };
        }

        public Vector Set(double x, double y)
        {
            X = x;
            Y = y;

            return this;
        }

        public Vector Set(Vector vector)
        {
            X = vector.X;
            Y = vector.Y;

            return this;
        }

        public double Dot(Vector vector)
        {
            return X * vector.X + Y * vector.Y;
        }

        public double Cross(Vector vector)
        {
            return X * vector.Y - Y * vector.X;
        }

        public Vector Plus(Vector vector)
        {
            X += vector.X;
            Y += vector.Y;

            return this;
        }

        public Vector PlusNew(Vector vector)
        {
            return new Vector() { X = X + vector.X, Y = Y + vector.Y };
        }

        public Vector Minus(Vector vector)
        {
            X -= vector.X;
            Y -= vector.Y;

            return this;
        }

        public Vector MinusNew(Vector vector)
        {
            return new Vector() { X = X - vector.X, Y = Y - vector.Y };
        }

        public Vector Mult(double scalar)
        {
            X *= scalar;
            Y *= scalar;

            return this;
        }

        public Vector MultNew(double scalar)
        {
            return new Vector() { X = X * scalar, Y = Y * scalar };
        }

        public double Distance(Vector vector)
        {
            double distanceX = X - vector.X;
            double distanceY = Y - vector.Y;

            return Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2));
        }

        public Vector Normalize()
        {
            double magnitude = Magnitude();

            if (magnitude is not 0)
            {
                X /= magnitude;
                Y /= magnitude;
            }

            return this;
        }

        public double Magnitude()
        {
            return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
        }

        public Vector Rotate(double angle)
        {
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);

            var x = X * cos - Y * sin;
            var y = X * sin + Y * cos;

            X = x;
            Y = y;

            return this;
        }
    }
}
