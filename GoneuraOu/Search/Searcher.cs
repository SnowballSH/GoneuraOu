using System;
using System.Diagnostics;
using System.Linq;
using GoneuraOu.Common;
using GoneuraOu.Evaluation;
using GoneuraOu.Logic;

namespace GoneuraOu.Search
{
    public partial class Searcher
    {
        public ulong Nodes;
        public int Ply;

        // id, ply
        public uint[,] KillerMoves;

        // pt, square
        public int[,] HistoryMoves;

        // pv
        public int[] PrincipalVariationLengths;
        public uint[,] PrincipalVariationTable;


        public Searcher()
        {
            KillerMoves = new uint[2, MaxPly];
            HistoryMoves = new int[20, 25];
            PrincipalVariationLengths = new int[MaxPly];
            PrincipalVariationTable = new uint[MaxPly, MaxPly];
        }

        public void Reset()
        {
            KillerMoves = new uint[2, MaxPly];
            HistoryMoves = new int[20, 25];
            PrincipalVariationLengths = new int[MaxPly];
            PrincipalVariationTable = new uint[MaxPly, MaxPly];
        }

        public void DoSearch(Board.Board board, uint depth)
        {
            Reset();

            IterativeDeepening(board, depth);

            Console.WriteLine($"bestmove {PrincipalVariationTable[0, 0].ToUci()}");
        }

        /// <summary>
        /// Iterative deepening search
        /// </summary>
        public void IterativeDeepening(Board.Board board, uint maxDepth)
        {
            var start = new Stopwatch();

            start.Start();

            for (var depth = 1u; depth <= maxDepth; depth++)
            {
                var score = Negamax(board, -7654321, 7654321, depth);

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
            }
        }

        public int Negamax(Board.Board board, int alpha, int beta, uint depth, bool doNull = true)
        {
            var foundPv = false;

            PrincipalVariationLengths[Ply] = Ply;

            if (depth == 0)
            {
                // Search for captures
                return Quiescence(board, alpha, beta);
            }

            if (Ply > MaxPly - 1)
            {
                return ClassicalEvaluation.Evaluate(board);
            }

            Nodes++;

            var incheck = board.IsMyKingAttacked(board.CurrentTurn);

            // Check extension
            if (incheck)
            {
                depth++;
            }

            if (!incheck)
            {
                var eval = board.Evaluate();

                // Razoring
                if (depth < 2 && eval + 350 <= alpha)
                {
                    return Quiescence(board, alpha, beta);
                }

                if (doNull)
                {
                    // Null Move Pruning
                    var r = 4 + Math.Min(3, depth / 4);
                    if (eval >= beta + 100)
                        r++;

                    r = Math.Min(r, depth);

                    if (depth >= 2)
                    {
                        board.MakeNullMove();
                        var score = -Negamax(board, -beta, -beta + 1,
                            (uint)Math.Max((int)depth - r, 0), false);
                        board.UndoNullMove();

                        if (score >= beta)
                        {
                            return score;
                        }
                    }
                }
            }

            var moves = board.GeneratePseudoLegalMoves().ToList();

            moves.Sort((x, y) =>
                board.ScoreMove(y, this).CompareTo(board.ScoreMove(x, this))
            );

            var legals = 0;
            foreach (var move in moves)
            {
                board.MakeMoveUnchecked(move);
                // Illegal move?
                if (board.IsMyKingAttacked(board.CurrentTurn.Invert()))
                {
                    board.UndoMove(move);
                    continue;
                }

                legals++;

                Ply++;

                int score;

                // PVS Search
                if (foundPv)
                {
                    if (legals >= FullDepthLimit && depth >= ReductionLimit
                                                 && move.GetCapture() == 0
                                                 && move.GetDrop() == 0
                                                 && PrincipalVariationTable[0, Ply] != move
                                                 && !incheck
                                                 && board.IsMyKingAttacked(board.CurrentTurn.Invert()))
                    {
                        score = -Negamax(board, -alpha - 1, -alpha, depth - 2);
                    }
                    else
                    {
                        score = alpha + 1;
                    }

                    if (score > alpha)
                    {
                        score = -Negamax(board, -alpha - 1, -alpha, depth - 1);

                        // Prove this position is good...
                        if (score > alpha && score < beta)
                        {
                            score = -Negamax(board, -beta, -alpha, depth - 1);
                        }
                    }
                }
                else
                {
                    score = -Negamax(board, -beta, -alpha, depth - 1);
                }

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

                    // PVS Enable
                    foundPv = true;

                    PrincipalVariationTable[Ply, Ply] = move;
                    for (var next = Ply + 1; next < PrincipalVariationLengths[Ply + 1]; next++)
                    {
                        PrincipalVariationTable[Ply, next] = PrincipalVariationTable[Ply + 1, next];
                    }

                    PrincipalVariationLengths[Ply] = PrincipalVariationLengths[Ply + 1];
                }
            }

            if (legals == 0)
            {
                return -987654 + Ply;
            }

            return alpha;
        }

        public int Quiescence(Board.Board board, int alpha, int beta)
        {
            Nodes++;

            var evaluation = board.Evaluate();

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