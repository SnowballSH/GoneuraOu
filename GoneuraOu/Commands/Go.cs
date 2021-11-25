using System;
using System.Diagnostics;
using GoneuraOu.Common;
using GoneuraOu.Logic;

namespace GoneuraOu.Commands
{
    public static class Go
    {
        public static void DoGo(Protocol proto, string[] tokens)
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

            uint first = 0;
            var moves = proto.CurrentPosition.GeneratePseudoLegalMoves();
            foreach (var m in moves)
            {
                proto.CurrentPosition.MakeMoveUnchecked(m);
                if (proto.CurrentPosition.IsMyKingAttacked(proto.CurrentPosition.CurrentTurn.Invert()))
                {
                    proto.CurrentPosition.UndoMove(m);
                }
                else
                {
                    if (first == 0)
                    {
                        first = m;
                    }

                    proto.CurrentPosition.UndoMove(m);
                    if (Random.Shared.NextSingle() < 0.7) continue;

                    Console.WriteLine($"bestmove {m.ToUci()}");
                    return;
                }
            }

            Console.WriteLine($"bestmove {first.ToUci()}");
        }
    }
}