using System;
using System.Diagnostics;
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

            searcher.DoSearch(proto.CurrentPosition, depth);

            GC.Collect();
        }
    }
}