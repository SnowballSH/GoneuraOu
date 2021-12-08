using GoneuraOu.Board;

namespace GoneuraOu.Logic
{
    public static class Threat
    {
        public static bool IsAttacked(this Board.Board board, int square, int turn)
        {
            var tt = turn * 10;
            return
                (Attacks.PawnAttacks[turn ^ 1, square] & board.Bitboards[(int)Piece.SentePawn + tt]) != 0
                || (Attacks.GoldAttacks[turn ^ 1, square] &
                    (board.Bitboards[(int)Piece.SenteGold + tt] | board.Bitboards[(int)Piece.SenteTokin + tt] |
                     board.Bitboards[(int)Piece.SentePromotedSilver + tt])) != 0
                || (Attacks.SilverAttacks[turn ^ 1, square] & board.Bitboards[(int)Piece.SenteSilver + tt]) != 0
                || (Attacks.GetRookAttacks(square, board.Occupancies[2]) &
                    (board.Bitboards[(int)Piece.SenteRook + tt] | board.Bitboards[(int)Piece.SenteDragon + tt])) != 0
                || (Attacks.GetBishopAttacks(square, board.Occupancies[2]) &
                    (board.Bitboards[(int)Piece.SenteBishop + tt] | board.Bitboards[(int)Piece.SenteHorse + tt])) != 0
                || (Attacks.KingAttacks[square] & (board.Bitboards[(int)Piece.SenteKing + tt] |
                                                   board.Bitboards[(int)Piece.SenteDragon + tt] |
                                                   board.Bitboards[(int)Piece.SenteHorse + tt])) != 0
                ;
        }

        public static uint AttackedBitBoard(this Board.Board board, int square, int turn)
        {
            var tt = turn * 10;
            return
                (Attacks.PawnAttacks[turn ^ 1, square] & board.Bitboards[(int)Piece.SentePawn + tt])
                | (Attacks.GoldAttacks[turn ^ 1, square] &
                   (board.Bitboards[(int)Piece.SenteGold + tt] | board.Bitboards[(int)Piece.SenteTokin + tt] |
                    board.Bitboards[(int)Piece.SentePromotedSilver + tt]))
                | (Attacks.SilverAttacks[turn ^ 1, square] & board.Bitboards[(int)Piece.SenteSilver + tt])
                | (Attacks.GetRookAttacks(square, board.Occupancies[2]) &
                   (board.Bitboards[(int)Piece.SenteRook + tt] | board.Bitboards[(int)Piece.SenteDragon + tt]))
                | (Attacks.GetBishopAttacks(square, board.Occupancies[2]) &
                   (board.Bitboards[(int)Piece.SenteBishop + tt] | board.Bitboards[(int)Piece.SenteHorse + tt]))
                | (Attacks.KingAttacks[square] & (board.Bitboards[(int)Piece.SenteKing + tt] |
                                                  board.Bitboards[(int)Piece.SenteDragon + tt] |
                                                  board.Bitboards[(int)Piece.SenteHorse + tt]))
                ;
        }
    }
}