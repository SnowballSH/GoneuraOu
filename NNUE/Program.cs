using Microsoft.ML;
using System;
using System.Linq;
using GoneuraOu.Board;
using GoneuraOu.Evaluation;
using NNUE;

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