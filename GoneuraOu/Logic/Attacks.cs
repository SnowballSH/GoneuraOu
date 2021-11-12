using System.Runtime.CompilerServices;
using GoneuraOu.Bitboard;
using GoneuraOu.Board;

namespace GoneuraOu.Logic
{
    public static class Attacks
    {
        /// --$--
        /// --P--
        /// -----
        public static readonly uint[,] PawnAttacks;

        /// -$$$-
        /// -$K$-
        /// -$$$-
        public static readonly uint[] KingAttacks;

        /// -$$$-
        /// -$G$-
        /// --$--
        public static readonly uint[] GoldAttacks;

        /// -$$$-
        /// --S--
        /// -$-$-
        public static readonly uint[] SilverAttacks;

        public static readonly uint[] BishopMasks;
        public static readonly uint[] RookMasks;

        public static readonly uint[,] BishopAttacks;
        public static readonly uint[,] RookAttacks;

        private static uint GeneratePawnAttacks(int square, Turn turn)
        {
            var bb = 0u.SetBitAt(square);
            uint attacks = 0;

            // Pawn on last rank is undefined behavior because you can never have a pawn on the last rank according to the rules.
            if (turn == Turn.Sente)
            {
                attacks |= bb >> Constants.BoardSize;
            }
            else
            {
                attacks |= bb << Constants.BoardSize;
            }

            return attacks;
        }

        private static uint GenerateGoldAttacks(int square)
        {
            var bb = 0u.SetBitAt(square);
            uint attacks = 0;

            // is not top rank
            if (square >= Constants.BoardSize)
            {
                // up
                attacks |= bb >> Constants.BoardSize;
                // up left
                if ((bb & Files.A) == 0)
                    attacks |= bb >> (Constants.BoardSize + 1);
                // up right
                if ((bb & Files.E) == 0)
                    attacks |= bb >> (Constants.BoardSize - 1);
            }

            // left
            if ((bb & Files.A) == 0)
                attacks |= bb >> 1;
            // right
            if ((bb & Files.E) == 0)
                attacks |= bb << 1;

            // is not bottom rank
            if (square < Constants.BoardArea - Constants.BoardSize)
            {
                // bottom
                attacks |= bb << Constants.BoardSize;
            }

            return attacks;
        }

        private static uint GenerateSilverAttacks(int square)
        {
            var bb = 0u.SetBitAt(square);
            uint attacks = 0;

            // is not top rank
            if (square >= Constants.BoardSize)
            {
                // up
                attacks |= bb >> Constants.BoardSize;
                // up left
                if ((bb & Files.A) == 0)
                    attacks |= bb >> (Constants.BoardSize + 1);
                // up right
                if ((bb & Files.E) == 0)
                    attacks |= bb >> (Constants.BoardSize - 1);
            }

            // is bottom rank
            if (square >= Constants.BoardArea - Constants.BoardSize) return attacks;

            // add bottom left
            if ((bb & Files.A) == 0)
                attacks |= bb << (Constants.BoardSize - 1);
            // add bottom right
            if ((bb & Files.E) == 0)
                attacks |= bb << (Constants.BoardSize + 1);

            return attacks;
        }

        private static uint GenerateKingAttacks(int square)
        {
            var bb = 0u.SetBitAt(square);
            var attacks = GenerateGoldAttacks(square);

            // is bottom rank
            if (square >= Constants.BoardArea - Constants.BoardSize) return attacks;

            // add bottom left
            if ((bb & Files.A) == 0)
                attacks |= bb << (Constants.BoardSize - 1);
            // add bottom right
            if ((bb & Files.E) == 0)
                attacks |= bb << (Constants.BoardSize + 1);

            return attacks;
        }

        public static uint GenerateBishopOccupancy(int square)
        {
            uint attacks = 0;

            int r, f;

            var tr = square / Constants.BoardSize;
            var tf = square % Constants.BoardSize;

            const int limit = Constants.BoardSize - 2;

            // top right
            for (r = tr + 1, f = tf + 1; r <= limit && f <= limit; r++, f++)
                attacks |= (uint)(1 << (r * Constants.BoardSize + f));
            // bottom right
            for (r = tr - 1, f = tf + 1; r >= 1 && f <= limit; r--, f++)
                attacks |= (uint)(1 << (r * Constants.BoardSize + f));
            // top left
            for (r = tr + 1, f = tf - 1; r <= limit && f >= 1; r++, f--)
                attacks |= (uint)(1 << (r * Constants.BoardSize + f));
            // bottom left
            for (r = tr - 1, f = tf - 1; r >= 1 && f >= 1; r--, f--)
                attacks |= (uint)(1 << (r * Constants.BoardSize + f));

            return attacks;
        }

        public static uint GenerateRookOccupancy(int square)
        {
            uint attacks = 0;

            int r, f;

            var tr = square / Constants.BoardSize;
            var tf = square % Constants.BoardSize;

            const int limit = Constants.BoardSize - 2;

            // down
            for (r = tr + 1; r <= limit; r++)
                attacks |= (uint)(1 << (r * Constants.BoardSize + tf));
            // up
            for (r = tr - 1; r >= 1; r--)
                attacks |= (uint)(1 << (r * Constants.BoardSize + tf));
            // right
            for (f = tf + 1; f <= limit; f++)
                attacks |= (uint)(1 << (tr * Constants.BoardSize + f));
            // left
            for (f = tf - 1; f >= 1; f--)
                attacks |= (uint)(1 << (tr * Constants.BoardSize + f));

            return attacks;
        }

        /// <summary>
        /// Generate bishop moves, but counting blocking pieces
        /// </summary>
        public static uint GenerateBishopAttacksOnTheFly(int square, uint block)
        {
            uint attacks = 0;

            int r, f;

            var tr = square / Constants.BoardSize;
            var tf = square % Constants.BoardSize;

            const int limit = Constants.BoardSize - 1;

            // top right
            for (r = tr + 1, f = tf + 1; r <= limit && f <= limit; r++, f++)
            {
                var k = (uint)(1 << (r * Constants.BoardSize + f));
                attacks |= k;
                if ((k & block) != 0) break;
            }

            // bottom right
            for (r = tr - 1, f = tf + 1; r >= 0 && f <= limit; r--, f++)
            {
                var k = (uint)(1 << (r * Constants.BoardSize + f));
                attacks |= k;
                if ((k & block) != 0) break;
            }

            // top left
            for (r = tr + 1, f = tf - 1; r <= limit && f >= 0; r++, f--)
            {
                var k = (uint)(1 << (r * Constants.BoardSize + f));
                attacks |= k;
                if ((k & block) != 0) break;
            }

            // bottom left
            for (r = tr - 1, f = tf - 1; r >= 0 && f >= 0; r--, f--)
            {
                attacks |= (uint)(1 << (r * Constants.BoardSize + f));
            }

            return attacks;
        }

        /// <summary>
        /// Generate rook moves, but counting blockers
        /// </summary>
        public static uint GenerateRookAttacksOnTheFly(int square, uint block)
        {
            uint attacks = 0;

            int r, f;

            var tr = square / Constants.BoardSize;
            var tf = square % Constants.BoardSize;

            const int limit = Constants.BoardSize - 1;

            // down
            for (r = tr + 1; r <= limit; r++)
            {
                var k = (uint)(1 << (r * Constants.BoardSize + tf));
                attacks |= k;
                if ((k & block) != 0) break;
            }

            // up
            for (r = tr - 1; r >= 0; r--)
            {
                var k = (uint)(1 << (r * Constants.BoardSize + tf));
                attacks |= k;
                if ((k & block) != 0) break;
            }

            // right
            for (f = tf + 1; f <= limit; f++)
            {
                var k = (uint)(1 << (tr * Constants.BoardSize + f));
                attacks |= k;
                if ((k & block) != 0) break;
            }

            // left
            for (f = tf - 1; f >= 0; f--)
            {
                var k = (uint)(1 << (tr * Constants.BoardSize + f));
                attacks |= k;
                if ((k & block) != 0) break;
            }

            return attacks;
        }

        /// <summary>
        /// Set Occupancies
        /// </summary>
        public static uint SetOccupancy(int index, int bitsInMask, uint attackMask)
        {
            uint occupancyMap = 0;

            for (var count = 0; count < bitsInMask; count++)
            {
                // get LSB1 index
                var square = attackMask.Lsb1();

                // we know attackMask[square] is always 1 (unless bb is empty)
                attackMask ^= square.SquareToBit();

                if ((index & (1 << count)) != 0)
                    occupancyMap |= (uint)1 << square;
            }

            return occupancyMap;
        }

        public static readonly int[] BishopRelevantBits =
        {
            3, 2, 2, 2, 3,
            2, 2, 2, 2, 2,
            2, 2, 4, 2, 2,
            2, 2, 2, 2, 2,
            3, 2, 2, 2, 3
        };

        public static readonly int[] RookRelevantBits =
        {
            6, 5, 5, 5, 6,
            5, 4, 4, 4, 5,
            5, 4, 4, 4, 5,
            5, 4, 4, 4, 5,
            6, 5, 5, 5, 6
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetBishopAttacks(int square, uint occupancy)
        {
            occupancy &= BishopMasks[square];
            occupancy *= Magic.BishopMagic[square];
            occupancy >>= 32 - BishopRelevantBits[square];

            return BishopAttacks[square, occupancy];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetRookAttacks(int square, uint occupancy)
        {
            occupancy &= RookMasks[square];
            occupancy *= Magic.RookMagic[square];
            occupancy >>= 32 - RookRelevantBits[square];

            return RookAttacks[square, occupancy];
        }

        static Attacks()
        {
            PawnAttacks = new uint[2, Constants.BoardArea];
            KingAttacks = new uint[Constants.BoardArea];
            GoldAttacks = new uint[Constants.BoardArea];
            SilverAttacks = new uint[Constants.BoardArea];
            BishopAttacks = new uint[Constants.BoardArea, Constants.MaxBishopOccupancy];
            RookAttacks = new uint[Constants.BoardArea, Constants.MaxRookOccupancy];

            BishopMasks = new uint[Constants.BoardArea];
            RookMasks = new uint[Constants.BoardArea];

            for (var square = 0;
                square < Constants.BoardArea;
                square++)
            {
                for (var turn = Turn.Sente; turn <= Turn.Gote; turn++)
                {
                    PawnAttacks[(int)turn, square] = GeneratePawnAttacks(square, turn);
                }

                KingAttacks[square] = GenerateKingAttacks(square);
                GoldAttacks[square] = GenerateGoldAttacks(square);
                SilverAttacks[square] = GenerateSilverAttacks(square);

                BishopMasks[square] = GenerateBishopOccupancy(square);
                RookMasks[square] = GenerateRookOccupancy(square);

                // bishop and rook attacks
                for (var bishop = 0; bishop < 2; bishop++)
                {
                    var attackMask = bishop == 1 ? BishopMasks[square] : RookMasks[square];

                    var relevantBits = attackMask.Count();
                    var occupancyIndices = 1 << relevantBits;

                    for (var i = 0; i < occupancyIndices; i++)
                    {
                        var occupancy = SetOccupancy(i, relevantBits, attackMask);
                        if (bishop == 1)
                        {
                            var magicIndex = (occupancy * Magic.BishopMagic[square]) >>
                                             (32 - BishopRelevantBits[square]);

                            BishopAttacks[square, magicIndex] = GenerateBishopAttacksOnTheFly(square, occupancy);
                        }
                        else
                        {
                            var magicIndex = (occupancy * Magic.RookMagic[square]) >>
                                             (32 - RookRelevantBits[square]);

                            RookAttacks[square, magicIndex] = GenerateRookAttacksOnTheFly(square, occupancy);
                        }
                    }
                }
            }
        }
    }
}