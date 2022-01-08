using System;
using System.Threading;
using GoneuraOu.Evaluation;

namespace GoneuraOu.Commands
{
    public struct SearchLimit
    {
        public uint? FixedDepth;
        public ulong? MyTime;
        public uint? MyInc;
        public ulong? MoveTime;
    }

    public static class StopSearch
    {
        public static bool Stop;
    }

    public class Protocol
    {
        public Board.Board CurrentPosition;
        public SearchLimit Limit;

        public Protocol()
        {
            CurrentPosition = new Board.Board();
            Limit = new SearchLimit();
        }

        public void StartProtocol()
        {
            Console.Out.Flush();

            Console.WriteLine("GoneuraOu Version 0.1");

            TranspositionTable.TranspositionTable.Init(16);

            Thread? mainThread = null;
            var initAttacks = true;

            while (true)
            {
                var input = Console.In.ReadLine()?.Trim();
                if (input == null) continue;
                var tokens = input.Split(' ');

                if (mainThread is {IsAlive: false})
                {
                    mainThread = null;
                }

                switch (tokens[0])
                {
                    case "isready":
                        if (initAttacks)
                        {
                            Perft.PerftInternal(CurrentPosition, 2);
                            initAttacks = false;
                        }

                        GC.Collect();

                        Console.WriteLine("readyok");
                        break;

                    case "uci":
                    case "usi":
                        Console.WriteLine("id name GoneuraOu");
                        Console.WriteLine("id author SnowballSH");
                        Console.WriteLine($"{tokens[0]}ok");

                        Console.WriteLine();

                        Console.WriteLine("option name Protocol type combo default uci var uci var usi");
                        Console.WriteLine("option name UCI_Variant type combo default minishogi var minishogi");

                        break;

                    case "ucinewgame":
                    case "usinewgame":
                        CurrentPosition = new Board.Board();
                        break;

                    case "go":
                        if (mainThread == null)
                        {
                            mainThread = this.DoGo(tokens);
                            GC.Collect();
                        }

                        break;

                    case "stop":
                        StopSearch.Stop = true;
                        break;

                    case "eval":
                        ClassicalEvaluation.Evaluate(CurrentPosition, true);
                        break;

                    case "d":
                    case "board":
                        CurrentPosition.PrintBoard();
                        break;

                    case "position":
                        this.DoPosition(tokens);
                        break;

                    case "setoption":
                        // todo
                        break;

                    case "exit":
                    case "quit":
                        GC.Collect();
                        return;
                }
            }
        }
    }
}