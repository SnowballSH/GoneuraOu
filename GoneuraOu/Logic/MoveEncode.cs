using System.Runtime.CompilerServices;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static uint EncodeMove(int source, int target, int pt, int promote, int drop, int capture)
        {
            return (uint)(source | (target << 5) | (pt << 10) | (promote << 15) | (drop << 16) | (capture << 17));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static uint GetSource(this uint move)
        {
            return move & 0x1F;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static uint GetTarget(this uint move)
        {
            return (move & 0x3e0) >> 5;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static uint GetPieceType(this uint move)
        {
            return (move & 0x7c00) >> 10;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static uint GetPromote(this uint move)
        {
            return (move & 0x8000) >> 15;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static uint GetDrop(this uint move)
        {
            return (move & 0x10000) >> 16;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static uint GetCapture(this uint move)
        {
            return (move & 0x20000) >> 17;
        }
    }
}