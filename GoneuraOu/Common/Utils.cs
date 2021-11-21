using System;
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

        public static void Display<T>(this T[,] matrix)
        {
            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                for (var j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j] + "\t");
                }

                Console.WriteLine();
            }
        }

        public static void Display<T>(this T[] matrix)
        {
            Console.Write("[");
            for (var i = 0; i < matrix.Length; i++)
            {
                Console.Write(matrix[i] + ", ");
            }
            Console.WriteLine("]");
        }
    }
}