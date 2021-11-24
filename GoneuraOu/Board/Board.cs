using System;
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
        public bool[,] PawnFiles = new bool[2, 5];

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
            Array.Clear(PawnFiles);

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
                            PawnFiles[0, file] = true;
                        }
                        else if (pt == Piece.GotePawn)
                        {
                            PawnFiles[1, file] = true;
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
            copy.PawnFiles = (bool[,])PawnFiles.Clone();
            return copy;
        }

        /// <summary>
        /// Performs the move on a new board
        /// </summary>
        /// <returns>new board?</returns>
        public Board? MakeMoveCopy(uint move)
        {
            var nb = ShallowCopy();

            var target = move.GetTarget();
            var pt = move.GetPieceType();
            var drop = move.GetDrop();

            // handle drop
            if (drop == 1)
            {
                var tt = (uint)nb.CurrentTurn * 10;
                Utils.ForceSetBit(ref nb.Bitboards[pt + tt], (int)target);
                Utils.ForceSetBit(ref nb.Occupancies[(int)nb.CurrentTurn], (int)target);
                nb.PieceLoc[target] = pt + tt;
                if (pt == (int)Piece.SentePawn)
                {
                    if (nb.CurrentTurn == Turn.Sente)
                    {
                        nb.PawnFiles[0, target % 5] = true;
                    }
                    else
                    {
                        nb.PawnFiles[1, target % 5] = true;
                    }
                }

                if (nb.Pocket[(int)nb.CurrentTurn, pt * 2])
                {
                    nb.Pocket[(int)nb.CurrentTurn, pt * 2] = false;
                }
                else
                {
                    nb.Pocket[(int)nb.CurrentTurn, pt * 2 + 1] = false;
                }
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

                // handle capture
                if (capture == 1)
                {
                    var plt = nb.PieceLoc[target].GetValueOrDefault(0);

                    Utils.ForcePopBit(ref nb.Bitboards[plt], (int)target);
                    Utils.ForcePopBit(ref nb.Occupancies[nb.CurrentTurn.InvertInt()], (int)target);

                    if (plt == (int)Piece.SentePawn)
                    {
                        nb.PawnFiles[0, target % 5] = false;
                    }
                    else if (plt == (int)Piece.GotePawn)
                    {
                        nb.PawnFiles[1, target % 5] = false;
                    }

                    if (nb.Pocket[(int)nb.CurrentTurn, Constants.CompressBasics[plt] * 2])
                    {
                        nb.Pocket[(int)nb.CurrentTurn, Constants.CompressBasics[plt] * 2 + 1] = true;
                    }
                    else
                    {
                        nb.Pocket[(int)nb.CurrentTurn, Constants.CompressBasics[plt] * 2] = true;
                    }
                }

                nb.PieceLoc[source] = null;
                nb.PieceLoc[target] = (uint)ppt;
            }

            nb.Occupancies[2] = nb.Occupancies[0] | nb.Occupancies[1];

            // Is king in check?
            if (nb.IsMyKingAttacked(nb.CurrentTurn))
            {
                return null;
            }

            nb.CurrentTurn = nb.CurrentTurn.Invert();

            return nb;
        }

        /// <summary>
        /// MakeMove, updating self
        /// </summary>
        public bool MakeMoveUnchecked(uint move)
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
                        PawnFiles[0, target % 5] = true;
                    }
                    else
                    {
                        PawnFiles[1, target % 5] = true;
                    }
                }

                if (Pocket[(int)CurrentTurn, pt * 2])
                {
                    Pocket[(int)CurrentTurn, pt * 2] = false;
                }
                else
                {
                    Pocket[(int)CurrentTurn, pt * 2 + 1] = false;
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
                        PawnFiles[0, target % 5] = false;
                    }
                    else if (plt == (int)Piece.GotePawn)
                    {
                        PawnFiles[1, target % 5] = false;
                    }

                    if (Pocket[(int)CurrentTurn, Constants.CompressBasics[plt] * 2])
                    {
                        Pocket[(int)CurrentTurn, Constants.CompressBasics[plt] * 2 + 1] = true;
                    }
                    else
                    {
                        Pocket[(int)CurrentTurn, Constants.CompressBasics[plt] * 2] = true;
                    }
                }

                PieceLoc[source] = null;
                PieceLoc[target] = (uint)ppt;
            }

            Occupancies[2] = Occupancies[0] | Occupancies[1];

            // Is king in check?
            if (IsMyKingAttacked(CurrentTurn))
            {
                CurrentTurn = CurrentTurn.Invert();
                return false;
            }

            CurrentTurn = CurrentTurn.Invert();

            return true;
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