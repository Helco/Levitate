using System;
using System.Runtime.InteropServices;

namespace Levitate
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public readonly struct IVector2
    {
        public static readonly IVector2 Zero = new IVector2(0, 0);
        public static readonly IVector2 One = new IVector2(1, 1);
        public static readonly IVector2 Right = new IVector2(1, 0);
        public static readonly IVector2 Left = new IVector2(-1, 0);
        public static readonly IVector2 Up = new IVector2(0, -1);
        public static readonly IVector2 Down = new IVector2(0, 1);

        public readonly int X, Y;

        public IVector2(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public override bool Equals(object? obj) => (obj is IVector2) ? ((IVector2)obj) == this : false;

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public override string ToString() => $"[{X},{Y}]";

        public IVector2 WithX(int newX) => new IVector2(newX, Y);
        public IVector2 WithY(int newY) => new IVector2(X, newY);

        public static IVector2 operator +(IVector2 a, IVector2 b) => new IVector2(a.X + b.X, a.Y + b.Y);
        public static IVector2 operator -(IVector2 a, IVector2 b) => new IVector2(a.X - b.X, a.Y - b.Y);
        public static IVector2 operator *(IVector2 a, int s) => new IVector2(a.X * s, a.Y * s);
        public static IVector2 operator /(IVector2 a, int s) => new IVector2(a.X / s, a.Y / s);
        public static IVector2 operator *(IVector2 a, IVector2 b) => new IVector2(a.X * b.X, a.Y * b.Y);
        public static IVector2 operator /(IVector2 a, IVector2 b) => new IVector2(a.X / b.X, a.Y / b.Y);
        public static bool operator ==(IVector2 a, IVector2 b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(IVector2 a, IVector2 b) => a.X != b.X || a.Y != b.Y;

        public IVector2 Normalized => this / GreatestCommonDivisor(X, Y);
        public int Dot(IVector2 a) => X * a.X + Y * a.Y;
        public int LengthSqr => Dot(this);
        public int Length => (int)Math.Sqrt(LengthSqr);
        public int LengthManhattan => Math.Abs(X) + Math.Abs(Y);

        // starting up and going clockwise
        public double Angle => Math.Atan2(X, -Y);
        public double AngleTo(IVector2 a) => (a - this).Angle;

        public IVector2 LeftOrthogonal => new IVector2(Y, -X);
        public IVector2 RightOrthogonal => new IVector2(-Y, X);
        public IVector2 Rotate90(int degrees)
        {
            if (degrees % 90 != 0)
                throw new ArgumentException("Degrees are not dividable by 90");
            int rotations = (degrees / 90) % 4;
            int absRotations = Math.Abs(rotations);
            return absRotations switch
            {
                0 => this,
                1 => rotations < 0 ? LeftOrthogonal : RightOrthogonal,
                2 => this * -1,
                3 => rotations < 0 ? RightOrthogonal : LeftOrthogonal,
                _ => throw new InvalidProgramException("how?")
            };
        }

        // ripped from wikipedia
        private int GreatestCommonDivisor(int a, int b)
        {
            int h;
            if (a == 0) return Math.Abs(b);
            if (b == 0) return Math.Abs(a);

            do
            {
                h = a % b;
                a = b;
                b = h;
            } while (b != 0);

            return Math.Abs(a);
        }
    }
}
