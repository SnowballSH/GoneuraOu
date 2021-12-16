using System;
using System.Collections.Generic;
using System.Linq;
using GoneuraOu.Board;
using GoneuraOu.Common;
using GoneuraOu.Logic;
using GoneuraOu.Search;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace NNUE
{
    public class FenEval
    {
        [LoadColumn(0)]
        public string Fen;
        [LoadColumn(1)]
        public float Eval;
    }
    
    public class BinEval
    {
        [VectorType((int)NeuralNetAdapt.NnSize)]
        public float[] Bin;
        public float Eval;
    }
    
    public class Prediction
    {
        [ColumnName("Score")]
        public float Eval;
    }
    
    public static class DataPrep
    {
        public static void Rec(Board board, Searcher searcher, List<string> data, int depth)
        {
            if (depth == 0)
            {
                return;
            }
            
            var moves = board.GeneratePseudoLegalMoves().ToList();
            moves.Sort((x, y) =>
                board.ScoreMove(y, searcher).CompareTo(board.ScoreMove(x, searcher))
            );
            foreach (var m in moves.GetRange(0, Math.Max(depth, 2)))
            {
                board.MakeMoveUnchecked(m);

                if (board.IsMyKingAttacked(board.CurrentTurn.Invert()))
                {
                    board.UndoMove(m);
                    continue;
                }

                var score = searcher.Negamax(board, -987654, 987654, 5);

                data.Add($"{board.ToFen()}," +
                         $"{score * (board.CurrentTurn == Turn.Sente ? 1 : -1)}");
                
                Rec(board, searcher, data, depth - 1);

                board.UndoMove(m);
            }
        }
        
        public static void Prep()
        {
            Console.WriteLine("Prepping...");
            var board = new Board();
            var searcher = new Searcher();

            var data = new List<string>();

            data.Add("Pos,Eval");

            Rec(board, searcher, data, 6);

            System.IO.File.WriteAllText("data.csv", string.Join('\n', data));
            
            Console.WriteLine("Done!");
        }
    }
}