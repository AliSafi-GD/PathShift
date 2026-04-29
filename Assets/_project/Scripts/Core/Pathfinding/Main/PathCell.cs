namespace _project.Scripts.Core.Pathfinding.Main
{
    public class PathCell
    {
        public int X;
        public int Y;

        public PathCell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override int GetHashCode() => (X, Y).GetHashCode();
        public override bool Equals(object obj)
        {
            if (obj is PathCell c)
                return c.X == X && c.Y == Y;
            return false;
        }
    }
}