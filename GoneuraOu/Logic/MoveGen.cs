using System;
using GoneuraOu.Bitboard;
using GoneuraOu.Board;

namespace GoneuraOu.Logic
{
    public static class MoveGen
    {
        public static void GenerateAllMoves(this Board.Board board)
        {
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
                    Console.WriteLine($"{Constants.SquareCoords[source]}{Constants.SquareCoords[target]}" +
                                      (promote ? "+" : ""));
                    sentePawnTargets ^= (uint)1 << target;
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
                    Console.WriteLine($"{Constants.SquareCoords[source]}{Constants.SquareCoords[target]}" +
                                      (promote ? "+" : ""));

                    gotePawnTargets ^= (uint)1 << target;
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
                        Console.WriteLine($"{Constants.SquareCoords[source]}{Constants.SquareCoords[target]}");
                        attacks ^= (uint)1 << target;
                    }

                    bits ^= (uint)1 << source;
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
                    while (attacks != 0)
                    {
                        var target = attacks.Lsb1();
                        Console.WriteLine($"{Constants.SquareCoords[source]}{Constants.SquareCoords[target]}");
                        attacks ^= (uint)1 << target;
                    }

                    bits ^= (uint)1 << source;
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
                        Console.WriteLine($"{Constants.SquareCoords[source]}{Constants.SquareCoords[target]}" +
                                          (promote ? "+" : ""));
                        attacks ^= (uint)1 << target;
                    }

                    bits ^= (uint)1 << source;
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
                        Console.WriteLine($"{Constants.SquareCoords[source]}{Constants.SquareCoords[target]}");
                        attacks ^= (uint)1 << target;
                    }

                    bits ^= (uint)1 << source;
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
                        Console.WriteLine($"{Constants.SquareCoords[source]}{Constants.SquareCoords[target]}" +
                                          (promote ? "+" : ""));
                        attacks ^= (uint)1 << target;
                    }

                    bits ^= (uint)1 << source;
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
                        Console.WriteLine($"{Constants.SquareCoords[source]}{Constants.SquareCoords[target]}" +
                                          (promote ? "+" : ""));
                        attacks ^= (uint)1 << target;
                    }

                    bits ^= (uint)1 << source;
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
                        Console.WriteLine($"{Constants.SquareCoords[source]}{Constants.SquareCoords[target]}");
                        attacks ^= (uint)1 << target;
                    }

                    bits ^= (uint)1 << source;
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
                        Console.WriteLine($"{Constants.SquareCoords[source]}{Constants.SquareCoords[target]}");
                        attacks ^= (uint)1 << target;
                    }

                    bits ^= (uint)1 << source;
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
                    freeBits ^= (uint)1 << target;

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

                    Console.WriteLine($"{Constants.AsciiPieces[pi / 2]}*{Constants.SquareCoords[target]}");
                }
            }
        }
    }
}