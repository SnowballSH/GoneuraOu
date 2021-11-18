﻿using System;
using System.Runtime.CompilerServices;
using GoneuraOu.Bitboard;
using GoneuraOu.Common;
using GoneuraOu.Logic;

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
        public uint?[] PieceLoc = new uint?[25];

        /*
         *  You can only have max 10 pieces per player in pocket:
         *  2 pawns 2 golds 2 silvers 2 rooks 2 bishops
         *  note that this is different from the 20 above
         */
        public bool[,] Pocket = new bool[2, 10];

        public Turn CurrentTurn;
        public (int, int) KingSquares;

        public const string StartingSFen = "rbsgk/4p/5/P4/KGSBR[-] w - 1";

        public Board()
        {
            LoadSFen(StartingSFen);
        }

        public Board(string sfen)
        {
            LoadSFen(sfen);
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
                        var pt = ((int)ch).ToPiece(promoteNext);

                        promoteNext = false;

                        var square = rank * Constants.BoardSize + file;
                        var sqbb = square.SquareToBit();

                        if (pt == Piece.SenteKing)
                        {
                            KingSquares.Item1 = square;
                        }
                        else if (pt == Piece.GoteKing)
                        {
                            KingSquares.Item2 = square;
                        }

                        Bitboards[(int)pt] |= sqbb;
                        Occupancies[(int)pt.PieceTurn()] |= sqbb;
                        Occupancies[2] |= sqbb;
                        PieceLoc[square] = (uint)pt;
                        file++;
                        break;
                }

                continue;

                parsePocket:
                if (ch is ']' or '-') break;
                var pocketPt = ((int)ch).ToPiece(false);
                if (!Pocket[(int)pocketPt.PieceTurn(), pocketPt.PieceType() * 2])
                {
                    Pocket[(int)pocketPt.PieceTurn(), pocketPt.PieceType() * 2] = true;
                }
                else
                {
                    Pocket[(int)pocketPt.PieceTurn(), pocketPt.PieceType() * 2 + 1] = true;
                }
            }

            if (parts.Length >= 2)
            {
                CurrentTurn = parts[1] == "w" ? Turn.Sente : Turn.Gote;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Board ShallowCopy()
        {
            var copy = (Board)MemberwiseClone();
            copy.Bitboards = (uint[])Bitboards.Clone();
            copy.Occupancies = (uint[])Occupancies.Clone();
            copy.Pocket = (bool[,])Pocket.Clone();
            copy.PieceLoc = (uint?[])PieceLoc.Clone();
            return copy;
        }

        /// <summary>
        /// Performs the move on a new board
        /// </summary>
        /// <returns>new board?</returns>
        public Board? MakeMove(uint move)
        {
            var nb = ShallowCopy();

            var target = move.GetTarget();
            var pt = move.GetPieceType();
            var drop = move.GetDrop();

            // handle drop
            if (drop == 1)
            {
                Utils.ForceSetBit(ref nb.Bitboards[pt], (int)target);
                Utils.ForceSetBit(ref nb.Occupancies[(int)nb.CurrentTurn], (int)target);
                nb.PieceLoc[target] = pt;
            }
            else
            {
                // lazy evaluation
                var source = move.GetSource();
                var promote = move.GetPromote();
                var capture = move.GetCapture();

                Utils.ForcePopBit(ref nb.Bitboards[pt], (int)source);
                Utils.ForcePopBit(ref nb.Occupancies[(int)nb.CurrentTurn], (int)source);

                var ppt = promote == 1 ? Constants.PromotesTo[pt] : (int)pt;
                Utils.ForceSetBit(ref nb.Bitboards[ppt], (int)target);
                Utils.ForceSetBit(ref nb.Occupancies[(int)nb.CurrentTurn], (int)target);

                if (ppt == (int)Piece.SenteKing)
                {
                    nb.KingSquares.Item1 = (int)target;
                }
                else if (ppt == (int)Piece.GoteKing)
                {
                    nb.KingSquares.Item2 = (int)target;
                }

                // handle capture
                if (capture == 1)
                {
                    Utils.ForcePopBit(ref nb.Bitboards[nb.PieceLoc[target].GetValueOrDefault(0)], (int)target);
                    Utils.ForcePopBit(ref nb.Occupancies[nb.CurrentTurn.InvertInt()], (int)target);
                }

                nb.PieceLoc[source] = null;
                nb.PieceLoc[target] = pt;

                // Is king in check?
                if (nb.IsAttacked(nb.CurrentTurn == Turn.Sente ? nb.KingSquares.Item1 : nb.KingSquares.Item2,
                    nb.CurrentTurn.InvertInt()))
                {
                    return null;
                }
            }

            nb.Occupancies[2] = nb.Occupancies[0] | nb.Occupancies[1];
            nb.CurrentTurn = nb.CurrentTurn.Invert();

            return nb;
        }

        public void PrintBoard()
        {
            for (var rank = 0; rank < Constants.BoardSize; rank++)
            {
                for (var file = 0; file < Constants.BoardSize; file++)
                {
                    Console.Write("+---");
                }

                Console.WriteLine('+');
                Console.Write('|');

                for (var file = 0; file < Constants.BoardSize; file++)
                {
                    var square = rank * Constants.BoardSize + file;

                    var piece = PieceLoc[square];

                    Console.Write(piece.HasValue ? Constants.AsciiPieces[piece.Value].PadLeft(2, ' ') : "  ");
                    Console.Write(" |");
                }

                Console.WriteLine($" {Constants.Alphabets[rank]}");
            }

            for (var file = 0; file < Constants.BoardSize; file++)
            {
                Console.Write("+---");
            }

            Console.WriteLine('+');
            Console.Write("  ");

            for (var file = 0; file < Constants.BoardSize; file++)
            {
                Console.Write(5 - file);
                Console.Write(file == Constants.BoardSize - 1 ? '\n' : "   ");
            }

            for (var turn = 0; turn < 2; turn++)
            {
                Console.Write($"{(Turn)turn} Pocket:");
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