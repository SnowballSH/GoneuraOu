using GoneuraOu.Bitboard;
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

        public static uint UsiToMove(this string str, Board.Board board)
        {
            if (str[1] == '*')
            {
                // drop
                var piece = ((int)str[0]).ToPiece(false) + (int)board.CurrentTurn * 10;
                var targetFile = 5 - (str[2] - '0');
                var targetRank = str[3] - 'a';
                var targetSq = targetRank * Constants.BoardSize + targetFile;
                return MoveEncode.EncodeMove(0, targetSq, (int)piece, 0, 1, 0);
            }
            else
            {
                // normal moves
                var sourceFile = 5 - (str[0] - '0');
                var sourceRank = str[1] - 'a';
                var targetFile = 5 - (str[2] - '0');
                var targetRank = str[3] - 'a';
                var promote = str.Length >= 5;
                var sourceSq = sourceRank * Constants.BoardSize + sourceFile;
                var targetSq = targetRank * Constants.BoardSize + targetFile;
                var capture = (board.Occupancies[board.CurrentTurn.InvertInt()] &
                               targetSq.SquareToBit()) != 0;
                return MoveEncode.EncodeMove(sourceSq, targetSq, (int)board.PieceLoc[sourceSq]!, promote ? 1 : 0, 0,
                    capture ? 1 : 0);
            }
        }
    }
}