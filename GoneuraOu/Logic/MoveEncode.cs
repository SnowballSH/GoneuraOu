namespace GoneuraOu.Logic
{
    public static class MoveEncode
    {
        /*
         * A Move will be represented by a uint (32-bit unsigned)
         * 0b00000000000000011111 Source                              0x1F
         * 0b00000000001111100000 Target                              0x3e0
         * 0b00000111110000000000 PieceType                           0x7c00
         * 0b00001000000000000000 PromoteFlag                         0x8000
         * 0b00010000000000000000 DropFlag (no source if enabled)     0x10000
         * 0b00100000000000000000 CaptureFlag                         0x20000
         */
    }
}