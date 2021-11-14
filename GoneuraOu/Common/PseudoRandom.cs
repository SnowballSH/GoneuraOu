using System.Runtime.CompilerServices;
using GoneuraOu.Board;

namespace GoneuraOu.Common
{
    public static class PseudoRandom
    {
        private static uint _state = 818418983;

        public static uint NextU32()
        {
            var n = _state;

            n ^= n << 13;
            n ^= n >> 17;
            n ^= n << 5;

            _state = n;

            return n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint NextU25()
        {
            // cut bits that are over 25
            return NextU32() & Bitboard.Bitboard.LegalBitboard;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint NextSplitU32()
        {
            return ((NextU32() & 0xFFFF) << 16) | (NextU32() & 0xFFFF);
        }
    }
}