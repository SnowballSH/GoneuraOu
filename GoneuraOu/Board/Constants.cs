namespace GoneuraOu.Board
{
    public static class Constants
    {
        public const byte BoardSize = 5;
        public const byte BoardArea = BoardSize * BoardSize;

        public const int MaxBishopOccupancy = 1 << 4;
        public const int MaxRookOccupancy = 1 << 6;

        public const int OccupancySize = MaxRookOccupancy;

        public static readonly char[] Alphabets = { 'a', 'b', 'c', 'd', 'e' };

        public static readonly string[] SquareCoords =
        {
            "a5", "b5", "c5", "d5", "e5",
            "a4", "b4", "c4", "d4", "e4",
            "a3", "b3", "c3", "d3", "e3",
            "a2", "b2", "c2", "d2", "e2",
            "a1", "b1", "c1", "d1", "e1",
        };

        public static readonly string[] AsciiPieces =
        {
            "P", "G", "S", "R", "B", "K", "+P", "+S", "+R", "+B",
            "p", "g", "s", "r", "b", "k", "+p", "+s", "+r", "+b"
        };
    }
}