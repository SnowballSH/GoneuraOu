using System;
using GoneuraOu.Board;
using GoneuraOu.Common;
using GoneuraOu.Logic;

namespace GoneuraOu.Bitboard
{
    public static class Magic
    {
        private static uint GenerateMagicCandidate()
        {
            return PseudoRandom.NextU32() & PseudoRandom.NextU32() & PseudoRandom.NextU32();
        }

        /// <summary>
        /// Find appropriate MAGIC Number
        /// https://www.chessprogramming.org/Looking_for_Magics
        /// </summary>
        private static uint FindMagic(int square, int relevantBits, bool isBishop)
        {
            uint[] occupancies = new uint[Constants.OccupancySize];
            uint[] attacks = new uint[Constants.OccupancySize];

            uint[] used = new uint[Constants.OccupancySize];

            var mask = isBishop ? Attacks.GenerateBishopOccupancy(square) : Attacks.GenerateRookOccupancy(square);

            var mi = 1 << relevantBits;

            for (var i = 0; i < mi; i++)
            {
                occupancies[i] = Attacks.SetOccupancy(i, relevantBits, mask);
                attacks[i] = isBishop
                    ? Attacks.GenerateBishopAttacksOnTheFly(square, occupancies[i])
                    : Attacks.GenerateRookAttacksOnTheFly(square, occupancies[i]);
            }

            // brute force
            for (var k = 0; k < 1000000; k++)
            {
                var magic = GenerateMagicCandidate();
                if (((magic * mask) & 0x1F00000).Count() < 3) continue;

                Array.Clear(used);

                var fail = false;
                for (var i = 0; !fail && i < mi; i++)
                {
                    var magicIndex = (occupancies[i] * magic) >> (32 - relevantBits);
                    if (used[magicIndex] == 0)
                        used[magicIndex] = occupancies[i];
                    else if (used[magicIndex] != occupancies[i])
                        fail = true;
                }

                if (!fail) return magic;
            }

            Console.Error.WriteLine("FAILED MAGIC GENERATION");

            return 0;
        }

        public static void PrintAllMagics()
        {
            Console.WriteLine("public static readonly uint[] BishopMagic = {");
            for (var sq = 0; sq < Constants.BoardArea; sq++)
            {
                Console.WriteLine($"    0x{FindMagic(sq, Attacks.BishopRelevantBits[sq], true):X},");
            }

            Console.WriteLine("};");

            Console.WriteLine("public static readonly uint[] RookMagic = {");
            for (var sq = 0; sq < Constants.BoardArea; sq++)
            {
                Console.WriteLine($"    0x{FindMagic(sq, Attacks.RookRelevantBits[sq], false):X},");
            }

            Console.WriteLine("};");
        }

        public static readonly uint[] BishopMagic =
        {
            0x48842203,
            0x41822404,
            0x81C86410,
            0x28912810,
            0xC0483021,
            0x2043032,
            0x8120C,
            0xC0880,
            0x18084302,
            0x400420C8,
            0x1048018,
            0x808040A0,
            0x412010,
            0x12826C4,
            0x50C3A007,
            0xD08608,
            0x4CD100,
            0x80E1800,
            0x2052080,
            0x121C008,
            0x2048E002,
            0xB4C08,
            0x40826053,
            0x963300,
            0x8502E400,
        };

        public static readonly uint[] RookMagic =
        {
            0x42080880,
            0x2044400,
            0x30116004,
            0x280A2020,
            0x1008A0C0,
            0x84208000,
            0x40504102,
            0x424000,
            0x50426002,
            0x802200,
            0x12090804,
            0x28841280,
            0x9842810,
            0x80C22202,
            0x108820,
            0x30442014,
            0x80906400,
            0x836000,
            0x134080,
            0x1040B040,
            0x4019081,
            0x1102220,
            0x810500,
            0x8110220,
            0x18881300,
        };
    }
}