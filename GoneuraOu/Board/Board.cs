using System;
using GoneuraOu.Bitboard;
using GoneuraOu.Common;

namespace GoneuraOu.Board
{
    /// <summary>
    /// Main Board class for MiniShogi
    /// There are 10 piece types:
    /// P, +P (moves like G),
    /// K,
    /// G,
    /// S, +S (moves like G),
    /// R, +R (moves like R + K),
    /// B, +B (moves like B + K),
    /// </summary>
    public class Board
    {
        public uint[] Bitboards = new uint[2 * 10];
        public uint[] Occupancies = new uint[3];

        /*
         *  You can only have max 10 pieces per player in pocket:
         *  2 pawns 2 golds 2 silvers 2 rooks 2 bishops
         *  note that this is different from the 20 above
         */
        public bool[,] Pocket = new bool[2, 10];

        public Turn CurrentTurn;

        public const string StartingSFen = "rbsgk/4p/5/P4/KGSBR[-] w - 1";

        public Board()
        {
            /*
                CurrentTurn = Turn.Sente;

                Bitboards[(int) Piece.SentePawn] = Square.A2.SquareToBit();
                Bitboards[(int) Piece.SenteKing] = Square.A1.SquareToBit();
                Bitboards[(int) Piece.SenteGold] = Square.B1.SquareToBit();
                Bitboards[(int) Piece.SenteSilver] = Square.C1.SquareToBit();
                Bitboards[(int) Piece.SenteBishop] = Square.D1.SquareToBit();
                Bitboards[(int) Piece.SenteRook] = Square.E1.SquareToBit();

                Bitboards[(int) Piece.GotePawn] = Square.E4.SquareToBit();
                Bitboards[(int) Piece.GoteKing] = Square.E5.SquareToBit();
                Bitboards[(int) Piece.GoteGold] = Square.D5.SquareToBit();
                Bitboards[(int) Piece.GoteSilver] = Square.C5.SquareToBit();
                Bitboards[(int) Piece.GoteBishop] = Square.B5.SquareToBit();
                Bitboards[(int) Piece.GoteRook] = Square.A5.SquareToBit();

                Occupancies[(int) Turn.Sente] = Ranks.One | Square.A2.SquareToBit();
                Occupancies[(int) Turn.Gote] = Ranks.Five | Square.E4.SquareToBit();
                Occupancies[2] = Occupancies[(int) Turn.Sente] | Occupancies[(int) Turn.Gote];
            */

            LoadSFen(StartingSFen);
        }

        public void LoadSFen(string sfen)
        {
            Array.Clear(Bitboards);
            Array.Clear(Occupancies);
            Array.Clear(Pocket);

            var promoteNext = false;
            var rank = 0;
            var file = 0;

            var parts = sfen.Split(' ');

            var pmode = false;

            foreach (var ch in parts[0])
            {
                if (pmode) goto parsePocket;
                switch (ch)
                {
                    case '/':
                        rank++;
                        file = 0;
                        break;
                    case >= '1' and <= '9':
                        file += ch - '0';
                        break;
                    case ' ':
                    case '[':
                        pmode = true;
                        break;
                    case '+':
                        promoteNext = true;
                        break;
                    default:
                        var pt = ((int) ch).ToPiece(promoteNext);
                        promoteNext = false;

                        var square = rank * Constants.BoardSize + file;
                        var sqbb = square.SquareToBit();

                        Bitboards[(int) pt] |= sqbb;
                        Occupancies[(int) pt.PieceTurn()] |= sqbb;
                        Occupancies[2] |= sqbb;
                        file++;
                        break;
                }

                continue;

                parsePocket:
                if (ch is ']' or '-') break;
                var pocketPt = ((int) ch).ToPiece(false);
                if (!Pocket[(int) pocketPt.PieceTurn(), pocketPt.PieceType() * 2])
                {
                    Pocket[(int) pocketPt.PieceTurn(), pocketPt.PieceType() * 2] = true;
                }
                else
                {
                    Pocket[(int) pocketPt.PieceTurn(), pocketPt.PieceType() * 2 + 1] = true;
                }
            }

            if (parts.Length >= 2)
            {
                CurrentTurn = parts[1] == "w" ? Turn.Sente : Turn.Gote;
            }
        }

        public void PrintBoard()
        {
            for (var rank = 0; rank < Constants.BoardSize; rank++)
            {
                for (var file = 0; file < Constants.BoardSize; file++)
                {
                    if (file == 0)
                    {
                        Console.Write($"{Constants.BoardSize - rank} ");
                    }

                    var square = rank * Constants.BoardSize + file;

                    int? piece = null;

                    for (var pi = 0; pi <= (int) Piece.GoteHorse; pi++)
                    {
                        var bb = Bitboards[pi];
                        if (!bb.GetBitAt(square)) continue;
                        piece = pi;
                        break;
                    }

                    Console.Write(piece.HasValue ? Constants.AsciiPieces[piece.Value].PadLeft(2, ' ') : " .");
                    Console.Write(file == Constants.BoardSize - 1 ? '\n' : ' ');
                }
            }

            Console.Write("   ");
            for (var file = 0; file < Constants.BoardSize; file++)
            {
                Console.Write(Constants.Alphabets[file]);
                Console.Write(file == Constants.BoardSize - 1 ? '\n' : "  ");
            }

            for (var turn = 0; turn < 2; turn++)
            {
                Console.Write($"{(Turn) turn} Pocket:");
                for (var pi = 0; pi < 10; pi++)
                {
                    if (!Pocket[turn, pi]) continue;

                    Console.Write(' ');
                    Console.Write(Constants.AsciiPieces[10 * turn + pi / 2]);
                }

                Console.WriteLine();
            }

            Console.WriteLine($"Turn: {CurrentTurn}");
        }
    }
}