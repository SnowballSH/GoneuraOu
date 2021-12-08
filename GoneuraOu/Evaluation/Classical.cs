using System.Linq;
using GoneuraOu.Bitboard;
using GoneuraOu.Board;
using GoneuraOu.Logic;

namespace GoneuraOu.Evaluation
{
    public static class ClassicalEvaluation
    {
        public static readonly int[] PieceValueSole =
        {
            100, // Pawn 
            353, // Gold
            300, // Silver
            470, // Rook
            426, // Bishop
            10000, // King
            340, // Tokin
            325, // Promoted Silver
            760, // Dragon
            720 // Horse
        };

        public static readonly int[] PocketValueSole =
        {
            120, // Pawn 
            400, // Gold
            365, // Silver
            740, // Rook
            695 // Bishop
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
            -4, 0, 2, 0, -4,
            0, 1, 0, 1, 0,
            2, 0, -2, 0, 2,
            0, 1, 0, 1, 0,
            -4, 0, 2, 0, -4,
        };

        private static readonly int[] PawnPsqT =
        {
            0, 0, 0, 0, 0,
            40, 40, 35, 40, 40,
            14, 14, 10, 14, 14,
            3, 3, 0, 3, 3,
            -5, -5, -7, -5, -5,
        };

        private static readonly int[] RookPsqT =
        {
            16, 4, 6, 4, 16,
            2, 6, 10, 6, 2,
            2, 8, 20, 8, 2,
            0, 4, 8, 4, 0,
            12, 0, 2, 0, 12,
        };

        private static readonly int[] BishopPsqT =
        {
            1, 0, 2, 0, 1,
            0, 5, 6, 5, 0,
            2, 6, 16, 6, 2,
            0, 5, 6, 5, 0,
            1, 0, 2, 0, 1,
        };

        public static int Evaluate(this Board.Board position)
        {
            var mul = -2 * (int)position.CurrentTurn + 1;

            var onboard = 0;
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
                            Piece.GoteKing => -KingPsqT[idx],
                            Piece.SentePawn => PawnPsqT[idx],
                            Piece.GotePawn => -PawnPsqT[FlipIndex[idx]],
                            Piece.SenteRook or Piece.SenteDragon => RookPsqT[idx],
                            Piece.GoteRook or Piece.GoteDragon => -RookPsqT[FlipIndex[idx]],
                            Piece.SenteBishop or Piece.SenteHorse => BishopPsqT[idx],
                            Piece.GoteBishop or Piece.GoteHorse => -BishopPsqT[FlipIndex[idx]],
                            _ => 0
                        };

                    onboard += material;
                    onboard += psqt;
                }

                idx++;
            }

            var hand = 0;

            for (var turn = 0; turn <= 1; turn++)
            {
                for (var pi = 0; pi < 5; pi++)
                {
                    if (position.Pocket[turn, pi])
                    {
                        hand += PocketValues[turn, pi];
                    }
                }
            }

            // Tempo
            const int tempo = 10;

            // // King Safety
            // var kingSafetyCount =
            //     (Attacks.KingAttacks[position.Bitboards[(uint)Piece.SenteKing].BitScan()] & ~position.Occupancies[2])
            //     .Count() -
            //     (Attacks.KingAttacks[position.Bitboards[(uint)Piece.GoteKing].BitScan()] & ~position.Occupancies[2])
            //     .Count();
            // var kingSafety = kingSafetyCount * 3;

            return (onboard + hand) * mul + tempo;
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