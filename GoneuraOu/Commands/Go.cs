using System;
using System.Diagnostics;
using GoneuraOu.Common;
using GoneuraOu.Evaluation;
using GoneuraOu.Logic;
using GoneuraOu.Search;

namespace GoneuraOu.Commands
{
    public static class Go
    {
        public static void DoGo(this Protocol proto, string[] tokens)
        {
            Debug.Assert(tokens[0] == "go");

            proto.Limit = new SearchLimit();

            var index = 1;
            while (index < tokens.Length)
            {
                switch (tokens[index++])
                {
                    case "depth":
                        proto.Limit.FixedDepth = uint.Parse(tokens[index++]);
                        break;

                    case "perft":
                        Perft.PerftRootPrint(proto.CurrentPosition, uint.Parse(tokens[index++]));
                        return;
                }
            }

            var searcher = new Searcher();

            var depth = proto.Limit.FixedDepth ?? 5;

            var score = searcher.Negamax(proto.CurrentPosition, -7654321, 7654321, depth);

            if (searcher.FinalBestMove != 0)
            {
                Console.WriteLine($"info depth {depth} score cp {score} nodes {searcher.Nodes}");
                Console.WriteLine($"bestmove {searcher.FinalBestMove.ToUci()}");
            }
        }
    }
}