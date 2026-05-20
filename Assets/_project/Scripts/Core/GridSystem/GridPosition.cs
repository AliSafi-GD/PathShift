using System;

namespace _project.Scripts.Core.GridSystem
{
    /// <summary>
    /// Pure logical coordinate on the grid. No world-space meaning.
    /// X = horizontal, Y = vertical (height/floor), Z = depth.
    /// </summary>
    [Serializable]
    public readonly struct GridPosition : IEquatable<GridPosition>
    {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }

        public GridPosition(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static GridPosition operator +(GridPosition a, GridPosition b)
            => new GridPosition(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static GridPosition operator -(GridPosition a, GridPosition b)
            => new GridPosition(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static bool operator ==(GridPosition a, GridPosition b) => a.Equals(b);
        public static bool operator !=(GridPosition a, GridPosition b) => !a.Equals(b);

        public bool Equals(GridPosition other) => X == other.X && Y == other.Y && Z == other.Z;
        public override bool Equals(object obj) => obj is GridPosition other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y, Z);
        public override string ToString() => $"({X}, {Y}, {Z})";
    }
}
