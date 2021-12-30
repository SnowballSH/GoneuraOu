using System;
using System.Runtime.CompilerServices;
using GoneuraOu.Board;
using GoneuraOu.Common;

namespace GoneuraOu.Bitboard
{
    public static class Kindergarten
    {
        private static readonly byte[][] RankAttacks;

        private static byte GenerateRankAttacks(byte square, byte block)
        {
            byte attacks = 0;

            int f;

            const int limit = Constants.BoardSize - 1;

            // right
            for (f = square + 1; f <= limit; f++)
            {
                var k = (byte)f.SquareToBit();
                attacks |= k;
                if ((k & block) != 0) break;
            }

            // left
            for (f = square - 1; f >= 0; f--)
            {
                var k = (byte)f.SquareToBit();
                attacks |= k;
                if ((k & block) != 0) break;
            }

            return attacks;
        }

        public static uint GetRankAttacks(int index, uint block)
        {
            return (uint)RankAttacks[index % 5][(block >> (index / 5 * 5)) & Ranks.Five] << (index / 5 * 5);
        }

        public static uint GetFileAttacks(int index, uint block)
        {
            var shifted = block >> (index % 5);
            var nb = shifted & 1;
            nb |= (shifted & ((int)Square.S5B).SquareToBit()) >> 4;
            nb |= (shifted & ((int)Square.S5C).SquareToBit()) >> 8;
            nb |= (shifted & ((int)Square.S5D).SquareToBit()) >> 12;
            nb |= (shifted & ((int)Square.S5E).SquareToBit()) >> 16;

            var attacks = (uint)RankAttacks[index / 5][nb];
            var final = attacks & 1;
            final |= (attacks & ((int)Square.S4A).SquareToBit()) << 4;
            final |= (attacks & ((int)Square.S3A).SquareToBit()) << 8;
            final |= (attacks & ((int)Square.S2A).SquareToBit()) << 12;
            final |= (attacks & ((int)Square.S1A).SquareToBit()) << 16;
            return final << (index % 5);
        }

        static Kindergarten()
        {
            RankAttacks = Utils.CreateJaggedArray<byte[][]>(5, 32);

            for (byte square = 0; square < 5; square++)
            {
                for (byte blocker = 0; blocker < 32; blocker++)
                {
                    RankAttacks[square][blocker] = GenerateRankAttacks(square, blocker);
                }
            }
        }
    }
}