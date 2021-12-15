using System;
using GoneuraOu.Bitboard;
using GoneuraOu.Board;
using GoneuraOu.Common;
using GoneuraOu.Logic;

namespace GoneuraOu.Evaluation
{
    public static class ClassicalEvaluation
    {
        public static readonly int[] PieceValueSole =
        {
            96, // Pawn 
            363, // Gold
            310, // Silver
            470, // Rook
            426, // Bishop
            10000, // King
            350, // Tokin
            335, // Promoted Silver
            760, // Dragon
            720 // Horse
        };

        public static readonly int[] PocketValueSole =
        {
            105, // Pawn 
            399, // Gold
            327, // Silver
            420, // Rook
            386 // Bishop
        };

        private static readonly int[] PieceValues;
        private static readonly int[,] PocketValues;

        private static readonly int[] FlipIndex =
        {
            20, 21, 22, 23, 24,
            15, 16, 17, 18, 19,
            10, 11, 12, 13, 14,
            5, 6, 7, 8, 9,
            0, 1, 2, 3, 4
        };

        private static readonly int[] KingPsqT =
        {
            10, 0, 2, 0, 14,
            0, -1, 0, -3, 0,
            2, 0, -5, 0, 2,
            0, -3, 0, -1, 0,
            14, 0, 2, 0, -10,
        };

        private static readonly int[] PawnPsqT =
        {
            0, 0, 0, 0, 0,
            20, 20, 17, 20, 20,
            14, 14, 11, 14, 14,
            3, 3, 0, 3, 3,
            -5, -5, -7, -5, -5,
        };

        private static readonly int[] GoldPsqT =
        {
            -7, -2, -1, -2, -7,
            -1, 3, 4, 3, -1,
            -1, 5, 6, 5, -1,
            -1, 2, 3, 2, -1,
            -10, -4, -1, -4, -10,
        };

        private static readonly int[] RookPsqT =
        {
            14, 4, 6, 4, 14,
            2, 6, 9, 6, 2,
            2, 8, 20, 8, 2,
            0, 4, 8, 4, 0,
            10, 0, 2, 0, 10,
        };

        private static readonly int[] BishopPsqT =
        {
            1, 0, 2, 0, 1,
            0, 5, 6, 5, 0,
            2, 6, 14, 6, 2,
            0, 5, 6, 5, 0,
            1, 0, 2, 0, 1,
        };

        public static int Evaluate(this Board.Board position, bool debug = false)
        {
            var mul = -2 * (int)position.CurrentTurn + 1;

            // todo: incremental update of king location
            var sking = position.Bitboards[(uint)Piece.SenteKing].BitScan();
            var gking = position.Bitboards[(uint)Piece.GoteKing].BitScan();

            var onboard = 0;
            var distanceSente = 0;
            var distanceGote = 0;

            var senteRookMobility = 0;
            var goteRookMobility = 0;

            var idx = 0;
            foreach (var p in position.PieceLoc)
            {
                if (p != null)
                {
                    var material = PieceValues[(uint)p];
                    var psqt =
                        (Piece)p switch
                        {
                            Piece.SenteKing => KingPsqT[idx],
                            Piece.GoteKing => -KingPsqT[FlipIndex[idx]],
                            Piece.SentePawn => PawnPsqT[idx],
                            Piece.GotePawn => -PawnPsqT[FlipIndex[idx]],
                            Piece.SenteRook or Piece.SenteDragon => RookPsqT[idx],
                            Piece.GoteRook or Piece.GoteDragon => -RookPsqT[FlipIndex[idx]],
                            Piece.SenteBishop or Piece.SenteHorse => BishopPsqT[idx],
                            Piece.GoteBishop or Piece.GoteHorse => -BishopPsqT[FlipIndex[idx]],
                            Piece.SenteGold or Piece.SenteTokin or Piece.SentePromotedSilver or Piece.SenteSilver =>
                                GoldPsqT[idx],
                            Piece.GoteGold or Piece.GoteTokin or Piece.GotePromotedSilver or Piece.GoteSilver =>
                                -GoldPsqT[FlipIndex[idx]],
                            _ => 0
                        } * 2 / 3;

                    onboard += material;
                    onboard += psqt;

                    switch ((Piece)p)
                    {
                        case Piece.SentePawn:
                        case Piece.SenteGold:
                        case Piece.SenteSilver:
                        case Piece.SenteTokin:
                        case Piece.SentePromotedSilver:
                            distanceSente -= Utils.DistanceBetween(sking, idx) - 3;
                            distanceSente += 7 - Utils.DistanceBetween(gking, idx);
                            break;

                        case Piece.GotePawn:
                        case Piece.GoteGold:
                        case Piece.GoteSilver:
                        case Piece.GoteTokin:
                        case Piece.GotePromotedSilver:
                            distanceGote -= Utils.DistanceBetween(gking, idx) - 3;
                            distanceGote += 7 - Utils.DistanceBetween(sking, idx);
                            break;

                        case Piece.SenteRook:
                            senteRookMobility += Attacks.GetRookAttacks(idx, position.Occupancies[2]).Count();
                            break;
                        case Piece.SenteDragon:
                            senteRookMobility += Attacks.GetDragonAttacks(idx, position.Occupancies[2]).Count();
                            break;

                        case Piece.GoteRook:
                            goteRookMobility += Attacks.GetRookAttacks(idx, position.Occupancies[2]).Count();
                            break;
                        case Piece.GoteDragon:
                            goteRookMobility += Attacks.GetDragonAttacks(idx, position.Occupancies[2]).Count();
                            break;
                    }
                }

                idx++;
            }

            // pocket

            var hand = 0;

            for (var turn = 0; turn <= 1; turn++)
            {
                for (var pi = 0; pi < 10; pi++)
                {
                    if (position.Pocket[turn, pi])
                    {
                        hand += PocketValues[turn, pi / 2];
                    }
                }
            }

            var distance = distanceSente - distanceGote;
            var mobility = senteRookMobility - goteRookMobility;

            // Tempo
            const int tempo = 30;

            // King Safety
            var kingSafetySente =
                ((Attacks.KingAttacks[sking] & position.Occupancies[0]).Count() -
                 Attacks.KingAttacks[sking].Count() / 2) * 7;
            var kingSafetyGote =
                ((Attacks.KingAttacks[gking] & position.Occupancies[1]).Count() -
                 Attacks.KingAttacks[gking].Count() / 2) * 7;
            var kingSafetyCount =
                kingSafetySente - kingSafetyGote;

            var sum = (onboard + hand + kingSafetyCount * 7 + distance * 7 + mobility * 3) * mul + tempo;

            if (debug)
            {
                Console.Write(
                    $"Material: {onboard}\n" +
                    $"Hand: {hand}\n" +
                    "---------------------\n" +
                    "Type           Sente Gote\n" +
                    $"King Safety:   {kingSafetySente * 7,-5} {kingSafetyGote * 7,-5}\n" +
                    $"Distance:      {distanceSente * 7,-5} {distanceGote * 7,-5}\n" +
                    $"Rook Mobility: {senteRookMobility * 3,-5} {goteRookMobility * 3,-5}\n" +
                    "---------------------\n" +
                    "For Current Player:\n" +
                    $"Tempo: {tempo}\n" +
                    "---------------------\n" +
                    $"Sum: {sum}\n"
                );
            }

            return sum;
        }

        public static void TunePiece(int p, int value)
        {
            PieceValueSole[p] = value;
            PieceValues[p] = value;
            PieceValues[p + 10] = -value;
        }

        public static void TunePocket(int p, int value)
        {
            PocketValueSole[p] = value;
            PocketValues[0, p] = value;
            PocketValues[1, p] = -value;
        }

        static ClassicalEvaluation()
        {
            PieceValues = new int[20];
            PocketValues = new int[2, 5];

            for (var i = 0; i < 10; i++)
            {
                PieceValues[i] = PieceValueSole[i];
                PieceValues[i + 10] = -PieceValueSole[i];
            }

            for (var turn = 0; turn <= 1; turn++)
            {
                var mul = -2 * turn + 1;
                for (var i = 0; i < 5; i++)
                {
                    PocketValues[turn, i] = PocketValueSole[i] * mul;
                }
            }
        }
    }
}