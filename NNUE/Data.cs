using System;
using System.Collections.Generic;
using System.Linq;
using GoneuraOu.Board;
using GoneuraOu.Common;
using GoneuraOu.Evaluation;
using GoneuraOu.Logic;
using GoneuraOu.Search;

namespace NNUE
{
    public static class DataPrep
    {
        public static int Rec(Board board, Searcher searcher, List<string> data, int depth)
        {
            if (depth == 0)
            {
                return searcher.Negamax(board, -987654, 987654, 6) * (board.CurrentTurn == Turn.Sente ? 1 : -1);
            }

            var moves = board.GeneratePseudoLegalMoves().ToList();
            foreach (var m in moves.GetRange(0, Math.Max(depth / 2, 2)))
            {
                board.MakeMoveUnchecked(m);

                if (board.IsMyKingAttacked(board.CurrentTurn.Invert()))
                {
                    board.UndoMove(m);
                    continue;
                }

                var score = searcher.Negamax(board, -987654, 987654, 5);
                var qscore = searcher.Quiescence(board, -987654, 987654);

                var factor = board.CurrentTurn == Turn.Sente ? 1 : -1;

                var idx = data.Count;
                data.Add($"{board.ToFen()};score:" +
                         $"{score * factor};eval:{board.Evaluate() * factor};qs:{qscore * factor};outcome:");

                var res = Rec(board, searcher, data, depth - 1);
                data[idx] += res > 130 ? "1.0" : res < -130 ? "0.0" : "0.5";

                board.UndoMove(m);
            }

            return searcher.Negamax(board, -987654, 987654, 6) * (board.CurrentTurn == Turn.Sente ? 1 : -1);
        }

        public static void Prep()
        {
            Console.WriteLine("Prepping...");
            var board = new Board();
            var searcher = new Searcher();

            var data = new List<string>();

            Rec(board, searcher, data, 6);

            System.IO.File.WriteAllText("data.txt", string.Join('\n', data));

            Console.WriteLine("Done!");
        }
    }
}