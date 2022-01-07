using System;
using System.Runtime.CompilerServices;
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
            80, // Pawn 
            339, // Gold
            279, // Silver
            400, // Rook
            356 // Bishop
        };

        private static readonly int[] PieceValues;

        private static readonly int[] KingPsqT =
        {
            10, 0, 2, 0, 14,
            0, -1, 0, -3, 0,
            2, 0, -5, 0, 2,
            0, -3, 0, -1, 0,
            14, 0, 2, 0, 10,
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

        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
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
            var senteBishopMobility = 0;
            var goteBishopMobility = 0;

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
                            Piece.GoteKing => -KingPsqT[24 - idx],
                            Piece.SentePawn => PawnPsqT[idx],
                            Piece.GotePawn => -PawnPsqT[24 - idx],
                            Piece.SenteRook or Piece.SenteDragon => RookPsqT[idx],
                            Piece.GoteRook or Piece.GoteDragon => -RookPsqT[24 - idx],
                            Piece.SenteBishop or Piece.SenteHorse => BishopPsqT[idx],
                            Piece.GoteBishop or Piece.GoteHorse => -BishopPsqT[24 - idx],
                            Piece.SenteGold or Piece.SenteTokin or Piece.SentePromotedSilver or Piece.SenteSilver =>
                                GoldPsqT[idx],
                            Piece.GoteGold or Piece.GoteTokin or Piece.GotePromotedSilver or Piece.GoteSilver =>
                                -GoldPsqT[24 - idx],
                            _ => 0
                        } * 4;

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

                        case Piece.SenteBishop:
                            senteBishopMobility += Attacks.GetBishopAttacks(idx, position.Occupancies[2]).Count();
                            break;
                        case Piece.SenteHorse:
                            senteBishopMobility += Attacks.GetHorseAttacks(idx, position.Occupancies[2]).Count();
                            break;

                        case Piece.GoteBishop:
                            goteBishopMobility += Attacks.GetBishopAttacks(idx, position.Occupancies[2]).Count();
                            break;
                        case Piece.GoteHorse:
                            goteBishopMobility += Attacks.GetHorseAttacks(idx, position.Occupancies[2]).Count();
                            break;
                    }
                }

                idx++;
            }

            // pocket

            var handSente = 0;
            var handGote = 0;

            for (var turn = 0; turn <= 1; turn++)
            {
                for (var pi = 0; pi < 10; pi++)
                {
                    if (position.Pocket[turn][pi])
                    {
                        if (turn == 0)
                        {
                            handSente += PocketValueSole[pi / 2];
                        }
                        else
                        {
                            handGote += PocketValueSole[pi / 2];
                        }
                    }
                }
            }

            var hand = handSente - handGote;

            var distance = distanceSente - distanceGote;
            var mobility = (senteRookMobility - goteRookMobility) * 3
                           + (senteBishopMobility - goteBishopMobility) * 2;

            // King Safety
            var kingSafetySente =
                ((Attacks.KingAttacks[sking] & position.Occupancies[0]).Count() -
                 Attacks.KingAttacks[sking].Count()) * 20;

            if (kingSafetySente < -50)
            {
                kingSafetySente -= handGote / 9;
            }
            else if (kingSafetySente < 20)
            {
                kingSafetySente -= handGote / 16;
            }
            else
            {
                kingSafetySente -= handGote / 50;
            }

            kingSafetySente -= goteRookMobility * 3;
            kingSafetySente -= goteBishopMobility * 3;

            if (position.Pocket[1][(int)Piece.SenteRook * 2])
            {
                if (kingSafetySente < -100)
                {
                    kingSafetySente -= kingSafetySente / 4 + 20;
                }
            }

            if (position.Pocket[1][(int)Piece.SenteRook * 2 + 1])
            {
                if (kingSafetySente < -100)
                {
                    kingSafetySente -= kingSafetySente / 4 + 20;
                }
            }

            if (position.Pocket[1][(int)Piece.SenteBishop * 2])
            {
                if (kingSafetySente < -100)
                {
                    kingSafetySente -= kingSafetySente / 4 + 5;
                }
            }

            if (position.Pocket[1][(int)Piece.SenteBishop * 2 + 1])
            {
                if (kingSafetySente < -100)
                {
                    kingSafetySente -= kingSafetySente / 4 + 5;
                }
            }

            var kingSafetyGote =
                ((Attacks.KingAttacks[gking] & position.Occupancies[1]).Count() -
                 Attacks.KingAttacks[gking].Count()) * 20;

            if (kingSafetyGote < -50)
            {
                kingSafetyGote -= handSente / 9;
            }
            else if (kingSafetyGote < 20)
            {
                kingSafetyGote -= handSente / 16;
            }
            else
            {
                kingSafetyGote -= handSente / 50;
            }

            kingSafetyGote -= senteRookMobility * 3;
            kingSafetyGote -= senteBishopMobility * 3;

            if (position.Pocket[0][(int)Piece.SenteRook * 2])
            {
                if (kingSafetyGote < -100)
                {
                    kingSafetyGote -= kingSafetyGote / 4 + 20;
                }
            }

            if (position.Pocket[0][(int)Piece.SenteRook * 2 + 1])
            {
                if (kingSafetyGote < -100)
                {
                    kingSafetyGote -= kingSafetyGote / 4 + 20;
                }
            }

            if (position.Pocket[0][(int)Piece.SenteBishop * 2])
            {
                if (kingSafetyGote < -100)
                {
                    kingSafetyGote -= kingSafetyGote / 4 + 5;
                }
            }

            if (position.Pocket[0][(int)Piece.SenteBishop * 2 + 1])
            {
                if (kingSafetySente < -100)
                {
                    kingSafetyGote -= kingSafetyGote / 4 + 5;
                }
            }

            var kingSafetyCount =
                kingSafetySente - kingSafetyGote;

            var sum = (onboard + hand + kingSafetyCount * 7 + distance * 7 + mobility * 2) * mul;

            const int tempo = 3;

            sum += tempo;

            if (debug)
            {
                Console.Write(
                    $"Material: {onboard}\n" +
                    "---------------------\n" +
                    "Type           Sente Gote\n" +
                    $"King Safety:   {kingSafetySente * 7,-5} {kingSafetyGote * 7,-5}\n" +
                    $"Distance:      {distanceSente * 7,-5} {distanceGote * 7,-5}\n" +
                    $"Mobility:      {senteRookMobility * 3 + senteBishopMobility * 2,-5} {goteRookMobility * 3 + goteBishopMobility * 2,-5}\n" +
                    $"Hand:          {handSente,-5} {handGote,-5}\n" +
                    "---------------------\n" +
                    "For Current Player:\n" +
                    $"Tempo: {tempo}\n" +
                    $"Sum: {sum}\n" +
                    "---------------------\n"
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
        }

        static ClassicalEvaluation()
        {
            PieceValues = new int[20];

            for (var i = 0; i < 10; i++)
            {
                PieceValues[i] = PieceValueSole[i];
                PieceValues[i + 10] = -PieceValueSole[i];
            }
        }
    }
}