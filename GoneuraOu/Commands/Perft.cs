using System;
using System.Diagnostics;
using GoneuraOu.Board;
using GoneuraOu.Common;
using GoneuraOu.Logic;

namespace GoneuraOu.Commands
{
    public static class Perft
    {
        public static void PerftRootPrint(this Board.Board board, uint depth)
        {
            var timer = new Stopwatch();
            timer.Start();

            var legalMoves = board.GeneratePseudoLegalMoves();
            var total = 0;
            foreach (var move in legalMoves)
            {
                var usi = move.ToUci();

#if DEBUG
                var oHash = board.Hash;
#endif

                board.MakeMoveUnchecked(move);

                if (board.IsMyKingAttacked(board.CurrentTurn.Invert())
                    || move.GetDrop() == 1
                    && (
                        move.GetPieceType() ==
                        (uint)Piece.SentePawn ||
                        move.GetPieceType() ==
                        (uint)Piece.GotePawn)
                    && board.IsMyKingAttacked(
                        board.CurrentTurn))
                {
                    board.UndoMove(move);
                    continue;
                }

                var nodes = board.PerftInternal(depth - 1);

                board.UndoMove(move);

#if DEBUG
                Debug.Assert(oHash == board.Hash, "Hash not equal");
#endif

                Console.WriteLine($"{usi}: {nodes}");
                total += nodes;
            }

            Console.WriteLine($"\nTotal Nodes: {total}");
            Console.WriteLine($"Total Time: {timer.ElapsedMilliseconds}ms");
            Console.WriteLine($"KNPS: {total / timer.Elapsed.TotalSeconds / 1000}");
        }

        public static int PerftInternal(this Board.Board board, uint depth)
        {
            if (depth == 0) return 1;

            var pseudoLegalMoves = board.GeneratePseudoLegalMoves();

            var nodes = 0;

            foreach (var move in pseudoLegalMoves)
            {
#if DEBUG
                var oHash = board.Hash;
#endif

                board.MakeMoveUnchecked(move);
                if (board.IsMyKingAttacked(board.CurrentTurn.Invert()))
                {
                }
                else if (move.GetDrop() == 1
                         && (
                             move.GetPieceType() == (uint)Piece.SentePawn ||
                             move.GetPieceType() == (uint)Piece.GotePawn)
                         && board.IsMyKingAttacked(board.CurrentTurn))
                {
                }
                else
                {
                    nodes += board.PerftInternal(depth - 1);
                }

                board.UndoMove(move);

#if DEBUG
                Debug.Assert(oHash == board.Hash, "Hash not equal");
#endif
            }

            return nodes;
        }
    }
}