using GoneuraOu.Bitboard;
using GoneuraOu.Board;

namespace GoneuraOu.Logic
{
    public static class Attacks
    {
        public static readonly uint[,] PawnAttacks;
        public static readonly uint[] KingAttacks;
        public static readonly uint[] GoldAttacks;
        public static readonly uint[] SilverAttacks;

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

        static Attacks()
        {
            PawnAttacks = new uint[2, Constants.BoardArea];
            KingAttacks = new uint[Constants.BoardArea];
            GoldAttacks = new uint[Constants.BoardArea];
            SilverAttacks = new uint[Constants.BoardArea];
            for (var square = 0; square < Constants.BoardArea; square++)
            {
                for (var turn = Turn.Sente; turn <= Turn.Gote; turn++)
                {
                    PawnAttacks[(int)turn, square] = GeneratePawnAttacks(square, turn);
                }

                KingAttacks[square] = GenerateKingAttacks(square);
                GoldAttacks[square] = GenerateGoldAttacks(square);
                SilverAttacks[square] = GenerateSilverAttacks(square);
            }
        }
    }
}