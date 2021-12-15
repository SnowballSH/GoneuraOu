using System;
using System.Collections.Generic;
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

        public static void Display<T>(this IEnumerable<T> matrix)
        {
            Console.Write("[");
            foreach (var t in matrix)
            {
                Console.Write(t + ", ");
            }
            Console.WriteLine("]");
        }

        public static void PrintBits(this byte b)
        {
            Console.WriteLine(Convert.ToString(b, 2).PadLeft(8, '0'));
        }

        public static int DistanceBetween(int a, int b)
        {
            var x1 = a / 5;
            var y1 = a % 5;
            var x2 = b / 5;
            var y2 = b % 5;

            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        }
    }
}