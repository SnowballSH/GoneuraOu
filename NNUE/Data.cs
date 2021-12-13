using System;
using GoneuraOu.Board;
using GoneuraOu.Logic;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace NNUE
{
    public static class DataPrep
    {
        public static void Prep()
        {
            var pos = new Board();
            var moves = pos.GeneratePseudoLegalMoves();
            foreach (var m in moves)
            {
                pos.MakeMoveUnchecked(m);
                Console.WriteLine(pos.ToFen());
                pos.UndoMove(m);
            }
        }
    }

    public class FenEval
    {
        [LoadColumn(0)] public string Fen;
        [LoadColumn(1)] public float Eval;
    }

    public class BinEval
    {
        [LoadColumn(0)]
        [VectorType(25 * 20 + 2 * 20)]
        public float[] Features;

        [LoadColumn(1)] public float Label;
    }

    public class Prediction
    {
        [ColumnName("Score")] public float Eval;
    }
}