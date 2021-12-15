using System;
using System.Diagnostics;
using System.Linq;
using GoneuraOu.Common;
using GoneuraOu.Evaluation;
using GoneuraOu.Logic;

namespace GoneuraOu.Search
{
    public class Searcher
    {
        public ulong Nodes;
        public int Ply;
        public uint FinalBestMove;

        // id, ply
        public uint[,] KillerMoves;

        // pt, square
        public int[,] HistoryMoves;

        // pv
        public int[] PrincipalVariationLengths;
        public uint[,] PrincipalVariationTable;

        public const uint MaxPly = 128;


        public Searcher()
        {
            KillerMoves = new uint[2, MaxPly];
            HistoryMoves = new int[20, 25];
            PrincipalVariationLengths = new int[MaxPly];
            PrincipalVariationTable = new uint[MaxPly, MaxPly];
        }

        public void DoSearch(Board.Board board, uint depth)
        {
            var start = new Stopwatch();

            start.Start();

            var score = Negamax(board, -7654321, 7654321, depth);

            if (FinalBestMove != 0)
            {
                Console.Write(
                    $"info depth {depth} score cp {score} nodes {Nodes} time {start.ElapsedMilliseconds} " +
                    $"nps {(start.ElapsedMilliseconds == 0 ? 0 : Nodes * 1000 / (ulong)start.ElapsedMilliseconds)} " +
                    "pv"
                );

                for (var c = 0; c < PrincipalVariationLengths[0]; c++)
                {
                    Console.Write(' ');
                    Console.Write(PrincipalVariationTable[0, c].ToUci());
                }

                Console.WriteLine();
                Console.WriteLine($"bestmove {FinalBestMove.ToUci()}");
            }
        }

        public int Negamax(Board.Board board, int alpha, int beta, uint depth)
        {
            PrincipalVariationLengths[Ply] = Ply;

            if (depth == 0)
            {
                return Quiescence(board, alpha, beta);
            }

            Nodes++;

            if (board.IsMyKingAttacked(board.CurrentTurn.Invert()))
            {
                depth++;
            }

            var bestMove = 0u;

            var moves = board.GeneratePseudoLegalMoves().ToList();

            moves.Sort((x, y) =>
                board.ScoreMove(y, this).CompareTo(board.ScoreMove(x, this))
            );

            var legals = 0;
            foreach (var move in moves)
            {
                board.MakeMoveUnchecked(move);
                if (board.IsMyKingAttacked(board.CurrentTurn.Invert()))
                {
                    board.UndoMove(move);
                    continue;
                }

                legals++;

                Ply++;

                var score = -Negamax(board, -beta, -alpha, depth - 1);

                board.UndoMove(move);
                Ply--;

                // fail-hard beta cutoff
                if (score >= beta)
                {
                    if (move.GetCapture() == 0)
                    {
                        // Killer Moves
                        KillerMoves[1, Ply] = KillerMoves[0, Ply];
                        KillerMoves[0, Ply] = move;
                    }

                    // fails high
                    return beta;
                }

                // better move
                if (score > alpha)
                {
                    if (move.GetCapture() == 0)
                    {
                        // History moves
                        HistoryMoves[move.GetPieceType(), move.GetTarget()] += (int)depth;
                    }

                    // PV
                    alpha = score;

                    PrincipalVariationTable[Ply, Ply] = move;
                    for (var next = Ply + 1; next < PrincipalVariationLengths[Ply + 1]; next++)
                    {
                        PrincipalVariationTable[Ply, next] = PrincipalVariationTable[Ply + 1, next];
                    }

                    PrincipalVariationLengths[Ply] = PrincipalVariationLengths[Ply + 1];

                    if (Ply == 0)
                    {
                        bestMove = move;
                    }
                }
            }

            if (legals > 0)
            {
                FinalBestMove = bestMove;
            }
            else
            {
                if (board.IsMyKingAttacked(board.CurrentTurn))
                {
                    // checkmate
                    return -987654 + Ply;
                }
                else
                {
                    // stalemate: loses
                    return -987654 + Ply;
                }
            }

            return alpha;
        }

        public int Quiescence(Board.Board board, int alpha, int beta)
        {
            Nodes++;

            var evaluation = ClassicalEvaluation.Evaluate(board);

            if (evaluation >= beta)
            {
                return beta;
            }

            if (evaluation > alpha)
            {
                alpha = evaluation;
            }

            var captures = board.GenerateCaptureMoves().ToList();

            if (captures.Count == 0)
            {
                return evaluation;
            }

            captures.Sort((x, y) =>
                board.ScoreMove(y, this).CompareTo(board.ScoreMove(x, this))
            );

            foreach (var move in captures)
            {
                board.MakeMoveUnchecked(move);
                if (board.IsMyKingAttacked(board.CurrentTurn.Invert()))
                {
                    board.UndoMove(move);
                    continue;
                }

                Ply++;

                var score = -Quiescence(board, -beta, -alpha);

                board.UndoMove(move);
                Ply--;

                // fail-hard beta cutoff
                if (score >= beta)
                {
                    // fails high
                    return beta;
                }

                // better move
                if (score > alpha)
                {
                    alpha = score;
                }
            }

            return alpha;
        }
    }
}