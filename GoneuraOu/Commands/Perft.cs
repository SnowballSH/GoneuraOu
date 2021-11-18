using System;
using System.Linq;
using GoneuraOu.Common;
using GoneuraOu.Logic;

namespace GoneuraOu.Commands
{
    public static class Perft
    {
        public static void PerftRootPrint(this Board.Board board, uint depth)
        {
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
        }

        public static int PerftInternal(this Board.Board board, uint depth)
        {
            if (depth == 0) return 1;
            var pseudoLegalMoves = board.GeneratePseudoLegalMoves();

            return pseudoLegalMoves
                .Select(move => board.MakeMove(move))
                .Sum(nb => nb?.PerftInternal(depth - 1) ?? 0);
        }
    }
}