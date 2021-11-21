namespace GoneuraOu.Board
{
    /// <summary>
    /// Btw Files and ranks are from Chess's perspective.
    /// </summary>
    public static class Files
    {
        public const uint A = 0b00001_00001_00001_00001_00001;
        public const uint B = 0b00010_00010_00010_00010_00010;
        public const uint C = 0b00100_00100_00100_00100_00100;
        public const uint D = 0b01000_01000_01000_01000_01000;
        public const uint E = 0b10000_10000_10000_10000_10000;
    }

    public static class Ranks
    {
        public const uint One = 0b11111_00000_00000_00000_00000;
        public const uint Two = 0b11111_00000_00000_00000;
        public const uint Three = 0b11111_00000_00000;
        public const uint Four = 0b11111_00000;
        public const uint Five = 0b11111;
    }
}