using System.Runtime.CompilerServices;
using GoneuraOu.Bitboard;
using GoneuraOu.Board;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Turn Invert(this Turn t)
        {
            return (Turn)((int)t ^ 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int InvertInt(this Turn t)
        {
            return (int)t ^ 1;
        }
    }
}