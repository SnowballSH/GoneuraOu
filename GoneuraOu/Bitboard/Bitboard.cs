using System.Numerics;
using System;
using System.Runtime.CompilerServices;
using GoneuraOu.Board;

namespace GoneuraOu.Bitboard
{
    // Implements uint (32-bit) for 5x5 board
    public static class Bitboard
    {
        public const uint LegalBitboard = 0x1ffffff;

        /// <summary>
        /// Gets the bit (1 or 0) at a given index
        /// </summary>
        /// <param name="bb">The bitboard</param>
        /// <param name="square">Index within [0, Constants.BoardArea)</param>
        /// <returns>true if bit is 1 otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetBitAt(this uint bb, int square)
        {
            return (bb & square.SquareToBit()) != 0;
        }

        /// <summary>
        /// Adds a bit (set to 1) to bitboard
        /// </summary>
        /// <param name="bb">The bitboard</param>
        /// <param name="square">Index within [0, Constants.BoardArea)</param>
        /// <returns>bit added to bb</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SetBitAt(this uint bb, int square)
        {
            return bb | square.SquareToBit();
        }

        /// <summary>
        /// Pops a bit (reset to 0) on a bitboard
        /// </summary>
        /// <param name="bb">The bitboard</param>
        /// <param name="square">Index within [0, Constants.BoardArea)</param>
        /// <returns>bit popped on bb</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint PopBitAt(this uint bb, int square)
        {
            return bb & ~square.SquareToBit();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SquareToBit(this int square)
        {
            return (uint)(1 << square);
        }

        /// <summary>
        /// Prettily displays a bitboard
        /// </summary>
        /// <param name="bb">bitboard represented by a u32 integer</param>
        public static void BitboardPrint(this uint bb)
        {
            for (var rank = 0; rank < Constants.BoardSize; rank++)
            {
                for (var file = 0; file < Constants.BoardSize; file++)
                {
                    // square index
                    var square = rank * Constants.BoardSize + file;

                    if (file == 0)
                    {
                        Console.Write($"{Constants.Alphabets[rank]} ");
                    }

                    Console.Write(bb.GetBitAt(square) ? 1 : 0);
                    Console.Write(file == Constants.BoardSize - 1 ? '\n' : ' ');
                }
            }

            Console.Write("  ");
            for (var file = 0; file < Constants.BoardSize; file++)
            {
                Console.Write(5 - file);
                Console.Write(file == Constants.BoardSize - 1 ? '\n' : ' ');
            }

            Console.WriteLine($"Decimal: {bb}\n");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count(this uint board)
        {
            return BitOperations.PopCount(board);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Lsb1(this uint board)
        {
            return BitOperations.TrailingZeroCount(board);
        }
    }
}