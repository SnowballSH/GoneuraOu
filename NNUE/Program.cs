using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using GoneuraOu.Board;
using NNUE;

void Mapping(FenEval input, BinEval output)
{
    output.Bin = new Board(input.Fen).ToNeuralNetFormat().Select(x => x ? 1.0f : 0.0f).ToArray();
    output.Eval = Math.Min(1000, Math.Max(-1000, input.Eval));
}

ITransformer Train(MLContext mlContext, string dataPath)
{
    var dataView = mlContext.Data.LoadFromTextFile<FenEval>(dataPath, hasHeader: true, separatorChar: ',');

    var pipeline =
        mlContext.Transforms.CustomMapping((Action<FenEval, BinEval>)Mapping, "tobin")
            .Append(mlContext.Transforms.CopyColumns("Label", "Eval"))
            .Append(mlContext.Transforms.Concatenate("Features", "Bin"))
            .Append(mlContext.Regression.Trainers.Sdca());

    var model = pipeline.Fit(dataView);
    mlContext.Model.Save(model, dataView.Schema, "dev.nn");
    return model;
}

void Evaluate(MLContext mlContext, string dataPath, ITransformer model)
{
    var dataView = mlContext.Data.LoadFromTextFile<FenEval>(dataPath, hasHeader: true, separatorChar: ',');

    var pipeline =
        mlContext.Transforms.CustomMapping((Action<FenEval, BinEval>)Mapping, "tobin")
            .Append(mlContext.Transforms.CopyColumns("Label", "Eval"))
            .Append(mlContext.Transforms.Concatenate("Features", "Bin"));

    var predictions = model.Transform(pipeline.Fit(dataView).Transform(dataView));

    var metrics = mlContext.Regression.Evaluate(predictions);

    Console.WriteLine();
    Console.WriteLine($"*************************************************");
    Console.WriteLine($"*       Model quality metrics evaluation         ");
    Console.WriteLine($"*------------------------------------------------");
    Console.WriteLine($"*       RSquared Score:      {metrics.RSquared:0.##}");
    Console.WriteLine($"*       Root Mean Squared Error:      {metrics.RootMeanSquaredError:#.##}");
}

// DataPrep.Prep();

var mlContext = new MLContext();

var model = Train(mlContext, "data.csv");
Evaluate(mlContext, "data.csv", model);