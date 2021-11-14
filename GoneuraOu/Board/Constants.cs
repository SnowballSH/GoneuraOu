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
            "5a", "4a", "3a", "2a", "1a",
            "5b", "4b", "3b", "2b", "1b",
            "5c", "4c", "3c", "2c", "1c",
            "5d", "4d", "3d", "2d", "1d",
            "5e", "4e", "3e", "2e", "1e",
        };

        public static readonly string[] AsciiPieces =
        {
            "P", "G", "S", "R", "B", "K", "+P", "+S", "+R", "+B",
            "p", "g", "s", "r", "b", "k", "+p", "+s", "+r", "+b"
        };
    }
}