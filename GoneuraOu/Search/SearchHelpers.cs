using GoneuraOu.Commands;

namespace GoneuraOu.Search
{
    public partial class Searcher
    {
        public const uint MaxPly = 128;
        public const uint FullDepthLimit = 4;
        public const uint ReductionLimit = 3;

        public const int Infinity = 7654321;

        public static ulong CalcTime(SearchLimit limit)
        {
            return limit.MoveTime - 40 ?? (limit.MyTime / 15 + limit.MyInc - 40 ?? 0);
        }
    }
}