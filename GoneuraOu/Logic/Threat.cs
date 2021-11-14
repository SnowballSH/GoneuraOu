using GoneuraOu.Board;

namespace GoneuraOu.Logic
{
    public static class Threat
    {
        public static bool IsAttacked(int square, int turn, Board.Board board)
        {
            return
                (Attacks.PawnAttacks[turn ^ 1, square] & board.Bitboards[(int)Piece.SentePawn + turn * 10]) != 0
                || (Attacks.GoldAttacks[square] & board.Bitboards[(int)Piece.SenteGold + turn * 10]) != 0
                || (Attacks.GoldAttacks[square] & board.Bitboards[(int)Piece.SenteTokin + turn * 10]) != 0
                || (Attacks.GoldAttacks[square] & board.Bitboards[(int)Piece.SentePromotedSilver + turn * 10]) != 0
                || (Attacks.SilverAttacks[square] & board.Bitboards[(int)Piece.SenteSilver + turn * 10]) != 0
                || (Attacks.GetRookAttacks(square, board.Occupancies[2]) &
                    board.Bitboards[(int)Piece.SenteRook + turn * 10]) != 0
                || (Attacks.GetBishopAttacks(square, board.Occupancies[2]) &
                    board.Bitboards[(int)Piece.SenteBishop + turn * 10]) != 0
                || (Attacks.KingAttacks[square] & board.Bitboards[(int)Piece.SenteKing + turn * 10]) != 0
                || (Attacks.GetDragonAttacks(square, board.Occupancies[2]) &
                    board.Bitboards[(int)Piece.SenteDragon + turn * 10]) != 0
                || (Attacks.GetHorseAttacks(square, board.Occupancies[2]) &
                    board.Bitboards[(int)Piece.SenteHorse + turn * 10]) != 0
                ;
        }

        public static uint AttackedBitBoard(int square, int turn, Board.Board board)
        {
            return
                (Attacks.PawnAttacks[turn ^ 1, square] & board.Bitboards[(int)Piece.SentePawn + turn * 10])
                | (Attacks.GoldAttacks[square] & board.Bitboards[(int)Piece.SenteGold + turn * 10])
                | (Attacks.GoldAttacks[square] & board.Bitboards[(int)Piece.SenteTokin + turn * 10])
                | (Attacks.GoldAttacks[square] & board.Bitboards[(int)Piece.SentePromotedSilver + turn * 10])
                | (Attacks.SilverAttacks[square] & board.Bitboards[(int)Piece.SenteSilver + turn * 10])
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