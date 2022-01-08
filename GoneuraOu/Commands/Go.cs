using System;
using System.Diagnostics;
using System.Threading;
using GoneuraOu.Board;
using GoneuraOu.Search;

namespace GoneuraOu.Commands
{
    public static class Go
    {
        public static Thread? DoGo(this Protocol proto, string[] tokens)
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

                    case "infinite":
                        proto.Limit.FixedDepth = Searcher.MaxPly;
                        break;

                    case "perft":
                        Perft.PerftRootPrint(proto.CurrentPosition, uint.Parse(tokens[index]));
                        return null;

                    case "wtime":
                        if (proto.CurrentPosition.CurrentTurn == Turn.Sente)
                            proto.Limit.MyTime = ulong.Parse(tokens[index++]);
                        break;

                    case "btime":
                        if (proto.CurrentPosition.CurrentTurn == Turn.Gote)
                            proto.Limit.MyTime = ulong.Parse(tokens[index++]);
                        break;

                    case "winc":
                        if (proto.CurrentPosition.CurrentTurn == Turn.Sente)
                            proto.Limit.MyInc = uint.Parse(tokens[index++]);
                        break;

                    case "binc":
                        if (proto.CurrentPosition.CurrentTurn == Turn.Gote)
                            proto.Limit.MyInc = uint.Parse(tokens[index++]);
                        break;

                    case "movetime":
                        proto.Limit.MoveTime = ulong.Parse(tokens[index++]);
                        break;
                }
            }

            StopSearch.Stop = false;
            var searcher = new Searcher();

            var th = new Thread(() => searcher.DoSearch(proto.CurrentPosition, proto.Limit));

            th.Start();

            GC.Collect();

            return th;
        }
    }
}