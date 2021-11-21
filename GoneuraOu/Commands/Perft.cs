using System;
using System.Diagnostics;
using System.Linq;
using GoneuraOu.Common;
using GoneuraOu.Logic;

namespace GoneuraOu.Commands
{
    public static class Perft
    {
        public static void PerftRootPrint(this Board.Board board, uint depth)
        {
            var timer = new Stopwatch();
            timer.Start();

            var legalMoves = board.GeneratePseudoLegalMoves();
            var total = 0;
            foreach (var move in legalMoves)
            {
                var usi = move.ToUsi();
                var nb = board.MakeMove(move);
                if (nb == null) continue;
                var nodes = nb.PerftInternal(depth - 1);
                Console.WriteLine($"{usi}: {nodes}");
                total += nodes;
            }

            Console.WriteLine($"\nTotal Nodes: {total}");
            Console.WriteLine($"Total Time: {timer.ElapsedMilliseconds}ms");
            Console.WriteLine($"KNPS: {total / timer.Elapsed.TotalSeconds / 1000}");
        }

        public static int PerftInternal(this Board.Board board, uint depth)
        {
            if (depth == 0) return 1;

            var pseudoLegalMoves = board.GeneratePseudoLegalMoves();

            var nodes = 0;

            foreach (var move in pseudoLegalMoves)
            {
                var nb = board.MakeMove(move);
                nodes += nb?.PerftInternal(depth - 1) ?? 0;
            }

            return nodes;
        }
    }
}