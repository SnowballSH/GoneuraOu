using System;
using System.Collections.Generic;
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
        public bool[][] Pocket = Utils.CreateJaggedArray<bool[][]>(2, 20);

        public Turn CurrentTurn;
        public bool[][] PawnFiles = Utils.CreateJaggedArray<bool[][]>(2, 5);

        public Stack<byte> CaptureHistory = new();

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
            Pocket = Utils.CreateJaggedArray<bool[][]>(2, 20);
            PawnFiles = Utils.CreateJaggedArray<bool[][]>(2, 5);
            Array.Clear(PieceLoc);
            CaptureHistory.Clear();

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

                        if (pt == Piece.SentePawn)
                        {
                            PawnFiles[0][file] = true;
                        }
                        else if (pt == Piece.GotePawn)
                        {
                            PawnFiles[1][file] = true;
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
                if (!Pocket[(int)pocketPt.PieceTurn()][pocketPt.PieceType() * 2])
                {
                    Pocket[(int)pocketPt.PieceTurn()][pocketPt.PieceType() * 2] = true;
                }
                else
                {
                    Pocket[(int)pocketPt.PieceTurn()][pocketPt.PieceType() * 2 + 1] = true;
                }
            }

            CurrentTurn = parts[1] == "w" ? Turn.Sente : Turn.Gote;
        }

        public string ToFen()
        {
            var fen = "";

            var connectedBlankCount = 0;

            for (var i = 0; i < 25; i++)
            {
                var p = PieceLoc[i];
                if (p.HasValue)
                {
                    if (connectedBlankCount > 0)
                    {
                        fen += connectedBlankCount.ToString();
                        connectedBlankCount = 0;
                    }

                    fen += Constants.AsciiPieces[p.Value];
                }
                else
                {
                    connectedBlankCount++;
                }

                if (i % 5 == 4 && i != 24)
                {
                    if (connectedBlankCount > 0)
                    {
                        fen += connectedBlankCount.ToString();
                        connectedBlankCount = 0;
                    }

                    fen += '/';
                }
            }

            fen += '[';
            for (var turn = 0; turn < 2; turn++)
            {
                for (var index = 0; index < 10; index++)
                {
                    var p = Pocket[turn][index];
                    if (p)
                    {
                        fen += Constants.AsciiPieces[index / 2 + turn * 10];
                    }
                }
            }

            fen += ']';

            fen += ' ';
            fen += CurrentTurn == Turn.Sente ? 'w' : 'b';

            return fen;
        }

        /// <summary>
        /// MakeMove, updating self
        /// </summary>
        public void MakeMoveUnchecked(uint move)
        {
            var target = move.GetTarget();
            var pt = move.GetPieceType();
            var drop = move.GetDrop();

            // handle drop
            if (drop == 1)
            {
                var tt = (uint)CurrentTurn * 10;
                Utils.ForceSetBit(ref Bitboards[pt + tt], (int)target);
                Utils.ForceSetBit(ref Occupancies[(int)CurrentTurn], (int)target);
                PieceLoc[target] = pt + tt;
                if (pt == (int)Piece.SentePawn)
                {
                    if (CurrentTurn == Turn.Sente)
                    {
                        PawnFiles[0][target % 5] = true;
                    }
                    else
                    {
                        PawnFiles[1][target % 5] = true;
                    }
                }

                if (Pocket[(int)CurrentTurn][pt * 2])
                {
                    Pocket[(int)CurrentTurn][pt * 2] = false;
                }
                else
                {
                    Pocket[(int)CurrentTurn][pt * 2 + 1] = false;
                }
            }
            else
            {
                // lazy evaluation
                var source = move.GetSource();
                var promote = move.GetPromote();
                var capture = move.GetCapture();

                Utils.ForcePopBit(ref Bitboards[pt], (int)source);
                Utils.ForcePopBit(ref Occupancies[(int)CurrentTurn], (int)source);

                var ppt = promote == 1 ? Constants.PromotesTo[pt] : (int)pt;
                Utils.ForceSetBit(ref Bitboards[ppt], (int)target);
                Utils.ForceSetBit(ref Occupancies[(int)CurrentTurn], (int)target);

                // handle capture
                if (capture == 1)
                {
                    var plt = PieceLoc[target].GetValueOrDefault(0);

                    Utils.ForcePopBit(ref Bitboards[plt], (int)target);
                    Utils.ForcePopBit(ref Occupancies[CurrentTurn.InvertInt()], (int)target);

                    if (plt == (int)Piece.SentePawn)
                    {
                        PawnFiles[0][target % 5] = false;
                    }
                    else if (plt == (int)Piece.GotePawn)
                    {
                        PawnFiles[1][target % 5] = false;
                    }

                    if (!Pocket[(int)CurrentTurn][Constants.CompressBasics[plt] * 2])
                    {
                        Pocket[(int)CurrentTurn][Constants.CompressBasics[plt] * 2] = true;
                    }
                    else
                    {
                        Pocket[(int)CurrentTurn][Constants.CompressBasics[plt] * 2 + 1] = true;
                    }

                    CaptureHistory.Push((byte)plt);
                }

                PieceLoc[source] = null;
                PieceLoc[target] = (uint)ppt;
            }

            Occupancies[2] = Occupancies[0] | Occupancies[1];

            CurrentTurn = CurrentTurn.Invert();
        }

        public void UndoMove(uint move)
        {
            CurrentTurn = CurrentTurn.Invert();

            var target = move.GetTarget();
            var pt = move.GetPieceType();
            var drop = move.GetDrop();

            // handle drop
            if (drop == 1)
            {
                var tt = (uint)CurrentTurn * 10;
                Utils.ForcePopBit(ref Bitboards[pt + tt], (int)target);
                Utils.ForcePopBit(ref Occupancies[(int)CurrentTurn], (int)target);

                PieceLoc[target] = null;

                if (pt == (int)Piece.SentePawn)
                {
                    if (CurrentTurn == Turn.Sente)
                    {
                        PawnFiles[0][target % 5] = false;
                    }
                    else
                    {
                        PawnFiles[1][target % 5] = false;
                    }
                }

                if (!Pocket[(int)CurrentTurn][pt * 2])
                {
                    Pocket[(int)CurrentTurn][pt * 2] = true;
                }
                else
                {
                    Pocket[(int)CurrentTurn][pt * 2 + 1] = true;
                }
            }
            else
            {
                // lazy evaluation
                var source = move.GetSource();
                var promote = move.GetPromote();
                var capture = move.GetCapture();

                Utils.ForceSetBit(ref Bitboards[pt], (int)source);
                Utils.ForceSetBit(ref Occupancies[(int)CurrentTurn], (int)source);

                var ppt = promote == 1 ? Constants.PromotesTo[pt] : (int)pt;
                Utils.ForcePopBit(ref Bitboards[ppt], (int)target);
                Utils.ForcePopBit(ref Occupancies[(int)CurrentTurn], (int)target);

                // handle capture
                if (capture == 1)
                {
                    var plt = (uint)CaptureHistory.Pop();

                    Utils.ForceSetBit(ref Bitboards[plt], (int)target);
                    Utils.ForceSetBit(ref Occupancies[CurrentTurn.InvertInt()], (int)target);

                    if (plt == (int)Piece.SentePawn)
                    {
                        PawnFiles[0][target % 5] = true;
                    }
                    else if (plt == (int)Piece.GotePawn)
                    {
                        PawnFiles[1][target % 5] = true;
                    }

                    if (Pocket[(int)CurrentTurn][Constants.CompressBasics[plt] * 2])
                    {
                        Pocket[(int)CurrentTurn][Constants.CompressBasics[plt] * 2] = false;
                    }
                    else
                    {
                        Pocket[(int)CurrentTurn][Constants.CompressBasics[plt] * 2 + 1] = false;
                    }

                    PieceLoc[target] = plt;
                }
                else
                {
                    PieceLoc[target] = null;
                }

                PieceLoc[source] = pt;
            }

            Occupancies[2] = Occupancies[0] | Occupancies[1];
        }

        public void MakeNullMove()
        {
            CurrentTurn = CurrentTurn.Invert();
        }

        public void UndoNullMove()
        {
            CurrentTurn = CurrentTurn.Invert();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMyKingAttacked(Turn turn)
        {
            return this.IsAttacked(
                turn == Turn.Sente
                    ? Bitboards[(int)Piece.SenteKing].BitScan()
                    : Bitboards[(int)Piece.GoteKing].BitScan(),
                turn.InvertInt());
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
                    if (!Pocket[turn][pi]) continue;

                    Console.Write(' ');
                    Console.Write(Constants.AsciiPieces[10 * turn + pi / 2]);
                }

                Console.WriteLine();
            }

            Console.WriteLine($"Turn: {CurrentTurn}");
        }
    }
}