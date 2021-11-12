using System;
using System.Runtime.CompilerServices;
using GoneuraOu.Board;

namespace GoneuraOu.Common
{
    public static class Conversion
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Turn PieceTurn(this Piece pt)
        {
            return pt >= Piece.GotePawn ? Turn.Gote : Turn.Sente;
        }

        public static Piece ToPiece(this int ch, bool promoted)
        {
            if (promoted)
            {
                return ch switch
                {
                    'P' => Piece.SenteTokin,
                    'p' => Piece.GoteTokin,
                    'S' => Piece.SentePromotedSilver,
                    's' => Piece.GotePromotedSilver,
                    'R' => Piece.SenteDragon,
                    'r' => Piece.GoteDragon,
                    'B' => Piece.SenteHorse,
                    'b' => Piece.GoteHorse,
                    _ => throw new Exception($"Invalid Piece: {ch}")
                };
            }

            return ch switch
            {
                'P' => Piece.SentePawn,
                'p' => Piece.GotePawn,
                'G' => Piece.SenteGold,
                'g' => Piece.GoteGold,
                'S' => Piece.SenteSilver,
                's' => Piece.GoteSilver,
                'R' => Piece.SenteRook,
                'r' => Piece.GoteRook,
                'B' => Piece.SenteBishop,
                'b' => Piece.GoteBishop,
                'K' => Piece.SenteKing,
                'k' => Piece.GoteKing,
                _ => throw new Exception($"Invalid Piece: {ch}")
            };
        }
    }
}