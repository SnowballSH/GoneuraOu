using GoneuraOu.Commands;

namespace GoneuraOu.Search
{
    public partial class Searcher
    {
        public const uint MaxPly = 128;
        public const uint FullDepthLimit = 2;
        public const uint ReductionLimit = 3;
        public const uint MoreReductionDepthLimit = 6;

        public const int Infinity = 7654321;
        public const int Checkmate = 987654;

        public static ulong CalcTime(SearchLimit limit)
        {
            return limit.MoveTime.HasValue
                ? limit.MoveTime.Value - 40
                : limit.MyTime.HasValue
                    ? limit.MyTime.Value / 20 + (limit.MyInc ?? 0) - 40
                    : ulong.MaxValue - 1000
                ;
        }
    }
}