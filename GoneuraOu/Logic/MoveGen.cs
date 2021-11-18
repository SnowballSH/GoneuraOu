using System;
using System.Collections.Generic;
using GoneuraOu.Bitboard;
using GoneuraOu.Board;
using GoneuraOu.Common;

namespace GoneuraOu.Logic
{
    public static class MoveGen
    {
        public static List<uint> GenerateAllMoves(this Board.Board board)
        {
            // define list of moves
            var moveList = new List<uint>();

            // Pawn
            // No worry about going off 25-bits: there must be no pawn on the last rank
            if (board.CurrentTurn == Turn.Sente)
            {
                var sentePawnTargets = (board.Bitboards[(int)Piece.SentePawn] >> Constants.BoardSize) &
                                       ~board.Occupancies[(int)Turn.Sente];
                while (sentePawnTargets != 0)
                {
                    var target = sentePawnTargets.Lsb1();
                    var source = target + Constants.BoardSize;
                    var promote = target <= (int)Square.S1A;
                    moveList.Add(MoveEncode.EncodeMove(source, target, (int)Piece.SentePawn,
                        Convert.ToInt32(promote), 0,
                        Convert.ToInt32(board.Occupancies[(int)Turn.Gote].GetBitAt(target))));

                    Utils.ForcePopBit(ref sentePawnTargets, target);
                }
            }
            else
            {
                var gotePawnTargets = (board.Bitboards[(int)Piece.GotePawn] << Constants.BoardSize) &
                                      ~board.Occupancies[2];
                while (gotePawnTargets != 0)
                {
                    var target = gotePawnTargets.Lsb1();
                    var source = target - Constants.BoardSize;
                    var promote = target >= (int)Square.S5E;
                    moveList.Add(MoveEncode.EncodeMove(source, target, (int)Piece.GotePawn,
                        Convert.ToInt32(promote), 0,
                        Convert.ToInt32(board.Occupancies[(int)Turn.Sente].GetBitAt(target))));

                    Utils.ForcePopBit(ref gotePawnTargets, target);
                }
            }

            // GOLD MOVES
            {
                var bits = board.Bitboards[(int)(board.CurrentTurn == Turn.Sente ? Piece.SenteGold : Piece.GoteGold)];
                while (bits != 0)
                {
                    var source = bits.Lsb1();
                    var attacks = Attacks.GoldAttacks[source] & ~board.Occupancies[(int)board.CurrentTurn];
                    while (attacks != 0)
                    {
                        var target = attacks.Lsb1();
                        moveList.Add(MoveEncode.EncodeMove(source, target,
                            (int)(board.CurrentTurn == Turn.Sente ? Piece.SenteGold : Piece.GoteGold),
                            0, 0,
                            Convert.ToInt32(board.Occupancies[(int)board.CurrentTurn ^ 1].GetBitAt(target))));
                        Utils.ForcePopBit(ref attacks, target);
                    }

                    Utils.ForcePopBit(ref bits, source);
                }
            }

            // PROMOTED GOLD-like MOVES
            {
                var bits = board.Bitboards[
                               (int)(board.CurrentTurn == Turn.Sente ? Piece.SenteTokin : Piece.GoteTokin)]
                           | board.Bitboards[
                               (int)(board.CurrentTurn == Turn.Sente
                                   ? Piece.SentePromotedSilver
                                   : Piece.GotePromotedSilver)];
                while (bits != 0)
                {
                    var source = bits.Lsb1();
                    var attacks = Attacks.GoldAttacks[source] & ~board.Occupancies[(int)board.CurrentTurn];
                    var pt = board
                        .Bitboards[(int)(board.CurrentTurn == Turn.Sente ? Piece.SenteTokin : Piece.GoteTokin)]
                        .GetBitAt(source)
                        ? board.CurrentTurn == Turn.Sente ? Piece.SenteTokin : Piece.GoteTokin
                        : board.CurrentTurn == Turn.Sente
                            ? Piece.SentePromotedSilver
                            : Piece.GotePromotedSilver;
                    while (attacks != 0)
                    {
                        var target = attacks.Lsb1();
                        moveList.Add(MoveEncode.EncodeMove(source, target,
                            (int)pt,
                            0, 0,
                            Convert.ToInt32(board.Occupancies[(int)board.CurrentTurn ^ 1].GetBitAt(target))));
                        Utils.ForcePopBit(ref attacks, target);
                    }

                    Utils.ForcePopBit(ref bits, source);
                }
            }

            // SILVER MOVES
            {
                var bits = board.Bitboards[
                    (int)(board.CurrentTurn == Turn.Sente ? Piece.SenteSilver : Piece.GoteSilver)];
                while (bits != 0)
                {
                    var source = bits.Lsb1();
                    var attacks = Attacks.SilverAttacks[source] & ~board.Occupancies[(int)board.CurrentTurn];
                    while (attacks != 0)
                    {
                        var target = attacks.Lsb1();
                        var promote = board.CurrentTurn == Turn.Sente
                            ? target <= (int)Square.S1A
                            : target >= (int)Square.S5E;
                        moveList.Add(MoveEncode.EncodeMove(source, target,
                            (int)(board.CurrentTurn == Turn.Sente ? Piece.SenteSilver : Piece.GoteSilver),
                            Convert.ToInt32(promote), 0,
                            Convert.ToInt32(board.Occupancies[(int)board.CurrentTurn ^ 1].GetBitAt(target))));
                        Utils.ForcePopBit(ref attacks, target);
                    }

                    Utils.ForcePopBit(ref bits, source);
                }
            }

            // KING MOVES
            {
                var bits = board.Bitboards[(int)(board.CurrentTurn == Turn.Sente ? Piece.SenteKing : Piece.GoteKing)];
                while (bits != 0)
                {
                    var source = bits.Lsb1();
                    var attacks = Attacks.KingAttacks[source] & ~board.Occupancies[(int)board.CurrentTurn];
                    while (attacks != 0)
                    {
                        var target = attacks.Lsb1();
                        moveList.Add(MoveEncode.EncodeMove(source, target,
                            (int)(board.CurrentTurn == Turn.Sente ? Piece.SenteKing : Piece.GoteKing),
                            0, 0,
                            Convert.ToInt32(board.Occupancies[(int)board.CurrentTurn ^ 1].GetBitAt(target))));
                        Utils.ForcePopBit(ref attacks, target);
                    }

                    Utils.ForcePopBit(ref bits, source);
                }
            }

            // ROOK MOVES
            {
                var bits = board.Bitboards[
                    (int)(board.CurrentTurn == Turn.Sente ? Piece.SenteRook : Piece.GoteRook)];
                while (bits != 0)
                {
                    var source = bits.Lsb1();
                    var attacks = Attacks.GetRookAttacks(source, board.Occupancies[2]) &
                                  ~board.Occupancies[(int)board.CurrentTurn];
                    while (attacks != 0)
                    {
                        var target = attacks.Lsb1();
                        var promote = board.CurrentTurn == Turn.Sente
                            ? target <= (int)Square.S1A
                            : target >= (int)Square.S5E;
                        moveList.Add(MoveEncode.EncodeMove(source, target,
                            (int)(board.CurrentTurn == Turn.Sente ? Piece.SenteRook : Piece.GoteRook),
                            Convert.ToInt32(promote), 0,
                            Convert.ToInt32(board.Occupancies[(int)board.CurrentTurn ^ 1].GetBitAt(target))));
                        Utils.ForcePopBit(ref attacks, target);
                    }

                    Utils.ForcePopBit(ref bits, source);
                }
            }

            // BISHOP MOVES
            {
                var bits = board.Bitboards[
                    (int)(board.CurrentTurn == Turn.Sente ? Piece.SenteBishop : Piece.GoteBishop)];
                while (bits != 0)
                {
                    var source = bits.Lsb1();
                    var attacks = Attacks.GetBishopAttacks(source, board.Occupancies[2]) &
                                  ~board.Occupancies[(int)board.CurrentTurn];
                    while (attacks != 0)
                    {
                        var target = attacks.Lsb1();
                        var promote = board.CurrentTurn == Turn.Sente
                            ? target <= (int)Square.S1A
                            : target >= (int)Square.S5E;
                        moveList.Add(MoveEncode.EncodeMove(source, target,
                            (int)(board.CurrentTurn == Turn.Sente ? Piece.SenteBishop : Piece.GoteBishop),
                            Convert.ToInt32(promote), 0,
                            Convert.ToInt32(board.Occupancies[(int)board.CurrentTurn ^ 1].GetBitAt(target))));
                        Utils.ForcePopBit(ref attacks, target);
                    }

                    Utils.ForcePopBit(ref bits, source);
                }
            }

            // DRAGON MOVES
            {
                var bits = board.Bitboards[
                    (int)(board.CurrentTurn == Turn.Sente ? Piece.SenteDragon : Piece.GoteDragon)];
                while (bits != 0)
                {
                    var source = bits.Lsb1();
                    var attacks = Attacks.GetDragonAttacks(source, board.Occupancies[2]) &
                                  ~board.Occupancies[(int)board.CurrentTurn];
                    while (attacks != 0)
                    {
                        var target = attacks.Lsb1();
                        moveList.Add(MoveEncode.EncodeMove(source, target,
                            (int)(board.CurrentTurn == Turn.Sente ? Piece.SenteDragon : Piece.GoteDragon),
                            0, 0,
                            Convert.ToInt32(board.Occupancies[(int)board.CurrentTurn ^ 1].GetBitAt(target))));
                        Utils.ForcePopBit(ref attacks, target);
                    }

                    Utils.ForcePopBit(ref bits, source);
                }
            }

            // HORSE MOVES
            {
                var bits = board.Bitboards[
                    (int)(board.CurrentTurn == Turn.Sente ? Piece.SenteHorse : Piece.GoteHorse)];
                while (bits != 0)
                {
                    var source = bits.Lsb1();
                    var attacks = Attacks.GetHorseAttacks(source, board.Occupancies[2]) &
                                  ~board.Occupancies[(int)board.CurrentTurn];
                    while (attacks != 0)
                    {
                        var target = attacks.Lsb1();
                        moveList.Add(MoveEncode.EncodeMove(source, target,
                            (int)(board.CurrentTurn == Turn.Sente ? Piece.SenteHorse : Piece.GoteHorse),
                            0, 0,
                            Convert.ToInt32(board.Occupancies[(int)board.CurrentTurn ^ 1].GetBitAt(target))));
                        Utils.ForcePopBit(ref attacks, target);
                    }

                    Utils.ForcePopBit(ref bits, source);
                }
            }

            // DROPS
            for (var pi = 0; pi < 10; pi += 2)
            {
                var inPocket = board.Pocket[(int)board.CurrentTurn, pi] ||
                               board.Pocket[(int)board.CurrentTurn, pi + 1];

                if (!inPocket) continue;

                var freeBits = ~board.Occupancies[2] & Bitboard.Bitboard.LegalBitboard;
                while (freeBits != 0)
                {
                    var target = freeBits.Lsb1();
                    Utils.ForcePopBit(ref freeBits, target);

                    // No pawn drops at last rank!
                    if (pi / 2 == (int)Piece.SentePawn)
                    {
                        if (board.CurrentTurn == Turn.Sente)
                        {
                            var inLastRank = target <= (int)Square.S1A;
                            if (inLastRank)
                                continue;
                        }
                        else
                        {
                            var inLastRank = target >= (int)Square.S5E;
                            if (inLastRank)
                                continue;
                        }
                    }

                    moveList.Add(MoveEncode.EncodeMove(0, target,
                        pi / 2,
                        0, 1, 0));
                }
            }

            return moveList;
        }
    }
}