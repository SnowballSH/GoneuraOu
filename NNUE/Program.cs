using Microsoft.ML;
using System;
using System.Linq;
using GoneuraOu.Board;
using GoneuraOu.Common;
using GoneuraOu.Evaluation;
using GoneuraOu.Logic;
using Microsoft.ML.Trainers;
using NNUE;

string NextPos(Board pos, uint depth)
{
    if (depth == 0)
    {
        return "";
    }

    var legals = pos.GeneratePseudoLegalMoves().ToList();
    var res = "";
    foreach (var move in legals.OrderBy(_ => Random.Shared.Next())
        .Skip(Math.Max(legals.Count - 2, 0)))
    {
        pos.MakeMoveUnchecked(move);
        if (pos.IsMyKingAttacked(pos.CurrentTurn.Invert()))
        {
            pos.UndoMove(move);
            continue;
        }

        res += '\n' + pos.ToFen() + ',' + pos.Evaluate() * (pos.CurrentTurn == Turn.Sente ? 1 : -1);

        res += NextPos(pos, depth - 1);

        pos.UndoMove(move);
    }

    return res;
}

var context = new MLContext();

void Inspect()
{
    var trainedModel = context.Model.Load("dev.nn", out _);

    Console.WriteLine("Ready to Evaluate.");

    while (true)
    {
        var line = Console.ReadLine();
        if (line == "quit")
        {
            break;
        }

        var board = new Board(line!.Trim());

        var predictionEngine =
            context.Model.CreatePredictionEngine<BinEval, Prediction>(trainedModel);

        var dat = new BinEval
        {
            Features = board.ToNeuralNetFormat().Select(x => (float)(x ? 1.0 : 0.0)).ToArray()
        };

        Console.WriteLine("NN, From White Perspective: " +
                          predictionEngine.Predict(dat).Eval);
        Console.WriteLine("Classical, From White Perspective: "
                          + board.Evaluate() * (board.CurrentTurn == Turn.Sente ? 1 : -1));
    }
}

Inspect();