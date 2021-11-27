using System;
using System.Diagnostics;
using GoneuraOu.Common;
using GoneuraOu.Evaluation;
using GoneuraOu.Logic;

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

                    default:
                        goto after;
                }
            }

            after:

            var moves = proto.CurrentPosition.GeneratePseudoLegalMoves();
            var bestMove = 0u;
            var bestScore = -10000000;
            foreach (var m in moves)
            {
                proto.CurrentPosition.MakeMoveUnchecked(m);
                if (proto.CurrentPosition.IsMyKingAttacked(proto.CurrentPosition.CurrentTurn.Invert()))
                {
                    proto.CurrentPosition.UndoMove(m);
                }
                else
                {
                    var eval = -proto.CurrentPosition.Evaluate();
                    if (eval > bestScore)
                    {
                        bestScore = eval;
                        bestMove = m;
                    }
                    proto.CurrentPosition.UndoMove(m);
                }
            }

            Console.WriteLine($"bestmove {bestMove.ToUci()}");
        }
    }
}