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
                if ((bb & Ranks.Five) == 0)
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
                if ((bb & Ranks.One) == 0)
                {
                    // bottom
                    attacks |= bb << Constants.BoardSize;
                }
            }
            else
            {
                // is not bottom rank
                if ((bb & Ranks.One) == 0)
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
                if ((bb & Ranks.Five) == 0)
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
                if ((bb & Ranks.Five) == 0)
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
                if ((bb & Ranks.One) != 0) return attacks;

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
                if ((bb & Ranks.One) == 0)
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
                if ((bb & Ranks.Five) != 0) return attacks;

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
            if ((bb & Ranks.One) != 0) return attacks;

            // add bottom left
            if ((bb & Files.A) == 0)
                attacks |= bb << (Constants.BoardSize - 1);
            // add bottom right
            if ((bb & Files.E) == 0)
                attacks |= bb << (Constants.BoardSize + 1);

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
                var k = (uint)(1 << (r * Constants.BoardSize + f));
                attacks |= k;
                if ((k & block) != 0) break;
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