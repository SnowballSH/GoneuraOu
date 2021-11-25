using System;

namespace GoneuraOu.Commands
{
    public struct SearchLimit
    {
        public uint? FixedDepth;

        public SearchLimit(uint? fixedDepth)
        {
            FixedDepth = fixedDepth;
        }
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

            Console.WriteLine("GoneuraOu Dev version 0.0");

            var initAttacks = true;

            while (true)
            {
                var input = Console.In.ReadLine()?.Trim();
                if (input == null) continue;
                var tokens = input.Split(' ');
                switch (tokens[0])
                {
                    case "isready":
                        if (initAttacks)
                        {
                            Perft.PerftInternal(new Board.Board(), 3);
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
                        Go.DoGo(this, tokens);
                        GC.Collect();
                        break;

                    case "position":
                        Position.DoPosition(this, tokens);
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