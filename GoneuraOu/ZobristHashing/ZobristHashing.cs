using System;
using System.Runtime.CompilerServices;
using GoneuraOu.Board;
using GoneuraOu.Common;

namespace GoneuraOu.ZobristHashing
{
    public static class ZobristHashing
    {
        // square, pt
        public static readonly uint[][] PieceHashes;

        // turn, ptnum
        public static readonly uint[][] HandHashes;
        public static readonly uint GoteTurn;

        private static Random ZRandom;
        private const int Seed = 24681357;

        private static uint RandomUInt()
        {
            return (uint)(ZRandom.Next(1 << 16) << 16) | (uint)Random.Shared.Next(1 << 16);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static uint ComputeHash(this Board.Board board)
        {
            uint hash = 0;

            for (var i = 0; i < 25; i++)
            {
                if (board.PieceLoc[i].HasValue)
                    hash ^= PieceHashes[i][board.PieceLoc[i]!.Value];
            }

            for (var t = 0; t < 2; t++)
            {
                for (var i = 0; i < 10; i += 2)
                {
                    var count = 0;
                    if (board.Pocket[t][i]) count++;
                    if (board.Pocket[t][i + 1]) count++;
                    if (count > 0)
                        hash ^= HandHashes[t][i + count - 1];
                }
            }

            if (board.CurrentTurn == Turn.Gote) hash ^= GoteTurn;

            return hash;
        }

        static ZobristHashing()
        {
            ZRandom = new Random(Seed);

            PieceHashes = Utils.CreateJaggedArray<uint[][]>(25, 20);
            HandHashes = Utils.CreateJaggedArray<uint[][]>(2, 20);

            GoteTurn = RandomUInt();

            for (var i = 0; i < 25; i++)
            {
                for (var j = 0; j < 20; j++)
                {
                    PieceHashes[i][j] = RandomUInt();
                }
            }

            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 20; j++)
                {
                    HandHashes[i][j] = RandomUInt();
                }
            }
        }
    }
}