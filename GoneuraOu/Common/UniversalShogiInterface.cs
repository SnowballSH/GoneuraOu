using GoneuraOu.Board;
using GoneuraOu.Logic;

namespace GoneuraOu.Common
{
    public static class UniversalShogiInterface
    {
        public static string ToUsi(this uint move)
        {
            if (move.GetDrop() == 1)
            {
                return $"{Constants.AsciiPieces[move.GetPieceType()]}*{Constants.SquareCoords[move.GetTarget()]}";
            }

            return $"{Constants.SquareCoords[move.GetSource()]}{Constants.SquareCoords[move.GetTarget()]}" +
                   (move.GetPromote() == 1 ? "+" : "");
        }
    }
}