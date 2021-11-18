using System.Runtime.CompilerServices;
using GoneuraOu.Bitboard;
using GoneuraOu.Board;
using GoneuraOu.Common;

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
        public static readonly uint[,] GoldAttacks;

        /// -$$$-
        /// --S--
        /// -$-$-
        public static readonly uint[,] SilverAttacks;

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

        private static uint GenerateGoldAttacks(int square, Turn turn)
        {
            var bb = 0u.SetBitAt(square);
            uint attacks = 0;

            if (turn == Turn.Sente)
            {
                // is not top rank
                if (square >= (int)Square.S5B)
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
                if (square <= (int)Square.S1D)
                {
                    // bottom
                    attacks |= bb << Constants.BoardSize;
                }
            }
            else
            {
                // is not bottom rank
                if (square <= (int)Square.S1D)
                {
                    // down
                    attacks |= bb << Constants.BoardSize;
                    // down left
                    if ((bb & Files.A) == 0)
                        attacks |= bb << (Constants.BoardSize - 1);
                    // down right
                    if ((bb & Files.E) == 0)
                        attacks |= bb << (Constants.BoardSize + 1);
                }

                // left
                if ((bb & Files.A) == 0)
                    attacks |= bb >> 1;
                // right
                if ((bb & Files.E) == 0)
                    attacks |= bb << 1;

                // is not top rank
                if (square >= (int)Square.S5B)
                {
                    // top
                    attacks |= bb >> Constants.BoardSize;
                }
            }

            return attacks;
        }

        private static uint GenerateSilverAttacks(int square, Turn turn)
        {
            var bb = 0u.SetBitAt(square);
            uint attacks = 0;

            if (turn == Turn.Sente)
            {
                // is not top rank
                if (square >= (int)Square.S5B)
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
                if (square >= (int)Square.S5E) return attacks;

                // add bottom left
                if ((bb & Files.A) == 0)
                    attacks |= bb << (Constants.BoardSize - 1);
                // add bottom right
                if ((bb & Files.E) == 0)
                    attacks |= bb << (Constants.BoardSize + 1);
            }
            else
            {
                // is not bottom rank
                if (square <= (int)Square.S1D)
                {
                    // bottom
                    attacks |= bb << Constants.BoardSize;
                    // bottom left
                    if ((bb & Files.A) == 0)
                        attacks |= bb << (Constants.BoardSize - 1);
                    // bottom right
                    if ((bb & Files.E) == 0)
                        attacks |= bb << (Constants.BoardSize + 1);
                }

                // is top rank
                if (square <= (int)Square.S1A) return attacks;

                // add top left
                if ((bb & Files.A) == 0)
                    attacks |= bb >> (Constants.BoardSize + 1);
                // add top right
                if ((bb & Files.E) == 0)
                    attacks |= bb >> (Constants.BoardSize - 1);
            }

            return attacks;
        }

        private static uint GenerateKingAttacks(int square)
        {
            var bb = 0u.SetBitAt(square);
            var attacks = GenerateGoldAttacks(square, Turn.Sente);

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
                Utils.ForceSetBit(ref attacks, r * Constants.BoardSize + f);
            // bottom right
            for (r = tr - 1, f = tf + 1; r >= 1 && f <= limit; r--, f++)
                Utils.ForceSetBit(ref attacks, r * Constants.BoardSize + f);
            // top left
            for (r = tr + 1, f = tf - 1; r <= limit && f >= 1; r++, f--)
                Utils.ForceSetBit(ref attacks, r * Constants.BoardSize + f);
            // bottom left
            for (r = tr - 1, f = tf - 1; r >= 1 && f >= 1; r--, f--)
                Utils.ForceSetBit(ref attacks, r * Constants.BoardSize + f);

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
                Utils.ForceSetBit(ref attacks, r * Constants.BoardSize + tf);
            // up
            for (r = tr - 1; r >= 1; r--)
                Utils.ForceSetBit(ref attacks, r * Constants.BoardSize + tf);
            ;
            // right
            for (f = tf + 1; f <= limit; f++)
                Utils.ForceSetBit(ref attacks, tr * Constants.BoardSize + f);
            // left
            for (f = tf - 1; f >= 1; f--)
                Utils.ForceSetBit(ref attacks, tr * Constants.BoardSize + f);

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
                Utils.ForceSetBit(ref attacks, r * Constants.BoardSize + f);
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
                Utils.ForcePopBit(ref attackMask, square);

                if ((index & (1 << count)) != 0)
                    Utils.ForceSetBit(ref occupancyMap, square);
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
            return GenerateBishopAttacksOnTheFly(square, occupancy);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetRookAttacks(int square, uint occupancy)
        {
            return GenerateRookAttacksOnTheFly(square, occupancy);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetDragonAttacks(int square, uint occupancy)
        {
            return GetRookAttacks(square, occupancy) | KingAttacks[square];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetHorseAttacks(int square, uint occupancy)
        {
            return GetBishopAttacks(square, occupancy) | KingAttacks[square];
        }

        static Attacks()
        {
            PawnAttacks = new uint[2, Constants.BoardArea];
            KingAttacks = new uint[Constants.BoardArea];
            GoldAttacks = new uint[2, Constants.BoardArea];
            SilverAttacks = new uint[2, Constants.BoardArea];

            for (var square = 0;
                square < Constants.BoardArea;
                square++)
            {
                for (var turn = Turn.Sente; turn <= Turn.Gote; turn++)
                {
                    PawnAttacks[(int)turn, square] = GeneratePawnAttacks(square, turn);
                    GoldAttacks[(int)turn, square] = GenerateGoldAttacks(square, turn);
                    SilverAttacks[(int)turn, square] = GenerateSilverAttacks(square, turn);
                }

                KingAttacks[square] = GenerateKingAttacks(square);
            }
        }
    }
}