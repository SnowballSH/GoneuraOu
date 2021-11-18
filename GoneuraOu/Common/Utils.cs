using System.Runtime.CompilerServices;
using GoneuraOu.Bitboard;

namespace GoneuraOu.Common
{
    public static class Utils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForcePopBit(ref uint bb, int square)
        {
            bb ^= square.SquareToBit();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForceSetBit(ref uint bb, int square)
        {
            bb |= square.SquareToBit();
        }
    }
}