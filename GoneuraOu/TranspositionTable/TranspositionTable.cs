using System;

// Credit: inspired from https://github.com/Tearth/Cosette
namespace GoneuraOu.TranspositionTable
{
    [Flags]
    public enum TranspositionFlag
    {
        Invalid = 0,
        Exact = 1,
        Alpha = 2,
        Beta = 4
    }

    public struct TranspositionEntry
    {
        public uint Key;
        public byte Depth;
        public TranspositionFlag Flags;
        public int Score;

        public TranspositionEntry(uint key, byte depth, TranspositionFlag flags, int score)
        {
            Key = key;
            Depth = depth;
            Flags = flags;
            Score = score;
        }
    }

    public static class TranspositionTable
    {
        private static TranspositionEntry[] _table;
        private static uint _size;

        static TranspositionTable()
        {
            unsafe
            {
                var entrySize = sizeof(TranspositionEntry);

                _size = 16 * 1024 * 1024 / (uint)entrySize;
                _table = new TranspositionEntry[_size];
            }
        }

        public static void Init(int sizeMegabytes)
        {
            Clear();
            unsafe
            {
                var entrySize = sizeof(TranspositionEntry);

                _size = (uint)sizeMegabytes * 1024 * 1024 / (uint)entrySize;
                _table = new TranspositionEntry[_size];
            }
        }

        public static void Add(uint hash, TranspositionEntry entry)
        {
            _table[hash % _size] = entry;
        }

        public static TranspositionEntry Get(uint hash)
        {
            return _table[hash % _size];
        }

        public static void Clear()
        {
            Array.Clear(_table, 0, (int)_size);
        }

        public static int RegularToTtScore(int score, int ply)
        {
            if (Math.Abs(score - Search.Searcher.Checkmate) < 100)
            {
                if (score > 0)
                {
                    return score + ply;
                }

                return score - ply;
            }

            return score;
        }

        public static int TtToRegularScore(int score, int ply)
        {
            if (Math.Abs(score - Search.Searcher.Checkmate) < 100)
            {
                if (score > 0)
                {
                    return score - ply;
                }

                return score + ply;
            }

            return score;
        }
    }
}