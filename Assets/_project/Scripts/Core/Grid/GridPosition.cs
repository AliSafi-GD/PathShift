namespace _project.Scripts.Domain.Grid
{
    public readonly struct GridPosition
    {
        public int X { get; }
        public int Y { get; }

        public GridPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static GridPosition operator +(GridPosition a, GridPosition b)
            => new GridPosition(a.X + b.X, a.Y + b.Y);

        public override bool Equals(object obj)
        {
            if (obj is not GridPosition other) return false;
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
            => (X, Y).GetHashCode();
    }
}