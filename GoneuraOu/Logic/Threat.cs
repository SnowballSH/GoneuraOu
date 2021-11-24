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
            return
                (Attacks.PawnAttacks[turn ^ 1, square] & board.Bitboards[(int)Piece.SentePawn + turn * 10])
                | (Attacks.GoldAttacks[turn ^ 1, square] & board.Bitboards[(int)Piece.SenteGold + turn * 10])
                | (Attacks.GoldAttacks[turn ^ 1, square] & board.Bitboards[(int)Piece.SenteTokin + turn * 10])
                | (Attacks.GoldAttacks[turn ^ 1, square] & board.Bitboards[(int)Piece.SentePromotedSilver + turn * 10])
                | (Attacks.SilverAttacks[turn ^ 1, square] & board.Bitboards[(int)Piece.SenteSilver + turn * 10])
                | (Attacks.GetRookAttacks(square, board.Occupancies[2]) &
                   board.Bitboards[(int)Piece.SenteRook + turn * 10])
                | (Attacks.GetBishopAttacks(square, board.Occupancies[2]) &
                   board.Bitboards[(int)Piece.SenteBishop + turn * 10])
                | (Attacks.KingAttacks[square] & board.Bitboards[(int)Piece.SenteKing + turn * 10])
                | (Attacks.GetDragonAttacks(square, board.Occupancies[2]) &
                   board.Bitboards[(int)Piece.SenteDragon + turn * 10])
                | (Attacks.GetHorseAttacks(square, board.Occupancies[2]) &
                   board.Bitboards[(int)Piece.SenteHorse + turn * 10])
                ;
        }
    }
}