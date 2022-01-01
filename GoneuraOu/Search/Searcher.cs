using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using GoneuraOu.Board;
using GoneuraOu.Commands;
using GoneuraOu.Common;
using GoneuraOu.Evaluation;
using GoneuraOu.Logic;
using GoneuraOu.TranspositionTable;

namespace GoneuraOu.Search
{
    public partial class Searcher
    {
        public ulong Nodes;
        public uint Ply;

        // id, ply
        public uint[][] KillerMoves;

        // pt, square
        public int[][] HistoryMoves;

        // pv
        public uint[] PrincipalVariationLengths;
        public uint[][] PrincipalVariationTable;

        private Stopwatch _timer;
        private ulong? _maxTime;


        public Searcher()
        {
            KillerMoves = Utils.CreateJaggedArray<uint[][]>(2, (int)MaxPly);
            HistoryMoves = Utils.CreateJaggedArray<int[][]>(20, 25);
            PrincipalVariationLengths = new uint[MaxPly];
            PrincipalVariationTable = Utils.CreateJaggedArray<uint[][]>((int)MaxPly, (int)MaxPly);
            _timer = new Stopwatch();
            _maxTime = null;
        }

        public void Reset()
        {
            KillerMoves = Utils.CreateJaggedArray<uint[][]>(2, (int)MaxPly);
            HistoryMoves = Utils.CreateJaggedArray<int[][]>(20, 25);
            PrincipalVariationLengths = new uint[MaxPly];
            PrincipalVariationTable = Utils.CreateJaggedArray<uint[][]>((int)MaxPly, (int)MaxPly);
            _timer = new Stopwatch();
            _maxTime = null;
        }

        public void DoSearch(Board.Board board, SearchLimit limit)
        {
            Reset();

            _maxTime = CalcTime(limit);
            _maxTime = _maxTime == 0 ? null : _maxTime;

            TranspositionTable.TranspositionTable.Clear();

            IterativeDeepening(board, limit.FixedDepth ?? MaxPly);

            Console.WriteLine($"bestmove {PrincipalVariationTable[0][0].ToUci()}");
        }

        /// <summary>
        /// Iterative deepening search
        /// </summary>
        public void IterativeDeepening(Board.Board board, uint maxDepth)
        {
            _timer.Start();

            var alpha = -Infinity;
            var beta = Infinity;

            const int window = 40;

            uint depthReached = 0;
            var score = 0;
            bool isMate;
            string scoreText;

            for (var depth = 1u; depth <= maxDepth; depth++)
            {
                var newScore = Negamax(board, alpha, beta, depth);

                if (_maxTime.HasValue && (ulong)_timer.ElapsedMilliseconds > _maxTime.Value)
                {
                    break;
                }

                score = newScore;

                isMate = Math.Abs(score - Checkmate) < 100;

                scoreText =
                    isMate ? $"mate {(score > 0 ? 1 : -1) * (Checkmate - Math.Abs(score))}" : $"cp {score}";

                Console.Write(
                    $"info depth {depth} score {scoreText} nodes {Nodes} time {_timer.ElapsedMilliseconds} " +
                    $"nps {(_timer.ElapsedMilliseconds == 0 ? 0 : Nodes * 1000 / (ulong)_timer.ElapsedMilliseconds)} " +
                    "pv"
                );

                for (var c = 0; c < PrincipalVariationLengths[0]; c++)
                {
                    Console.Write(' ');
                    Console.Write(PrincipalVariationTable[0][c].ToUci());
                }

                Console.WriteLine();

                depthReached = depth;

                if (isMate)
                {
                    break;
                }

                if (score <= alpha || score >= beta)
                {
                    alpha = -Infinity;
                    beta = Infinity;
                    continue;
                }

                alpha = score - window;
                beta = score + window;
            }

            _timer.Stop();

            isMate = Math.Abs(score - Checkmate) < 100;

            scoreText =
                isMate ? $"mate {(score > 0 ? 1 : -1) * (Checkmate - Math.Abs(score))}" : $"cp {score}";

            Console.Write(
                $"info depth {depthReached} score {scoreText} nodes {Nodes} time {_timer.ElapsedMilliseconds} " +
                $"nps {(_timer.ElapsedMilliseconds == 0 ? 0 : Nodes * 1000 / (ulong)_timer.ElapsedMilliseconds)} " +
                "pv"
            );

            for (var c = 0; c < PrincipalVariationLengths[0]; c++)
            {
                Console.Write(' ');
                Console.Write(PrincipalVariationTable[0][c].ToUci());
            }

            Console.WriteLine();
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public int Negamax(Board.Board board, int alpha, int beta, uint depth, bool doNull = true)
        {
#if DEBUG
            var oHash = board.Hash;
#endif
            var foundPv = false;
            var originalAlpha = alpha;

            PrincipalVariationLengths[Ply] = Ply;

            if (Ply > MaxPly - 1)
            {
                return board.Evaluate();
            }

            Nodes++;

            var entry = TranspositionTable.TranspositionTable.Get(board.Hash);

            if (entry.Flags != TranspositionFlag.Invalid && entry.Key == board.Hash)
            {
                if (entry.Depth >= depth)
                {
                    if (entry.Flags == TranspositionFlag.Exact)
                    {
                        return TranspositionTable.TranspositionTable.TtToRegularScore(entry.Score, (int)Ply);
                    }

                    if (entry.Flags == TranspositionFlag.Beta)
                    {
                        alpha = Math.Max(alpha, entry.Score);
                    }

                    else if (entry.Flags == TranspositionFlag.Alpha)
                    {
                        beta = Math.Min(beta, entry.Score);
                    }

                    if (alpha >= beta)
                    {
                        return TranspositionTable.TranspositionTable.TtToRegularScore(entry.Score, (int)Ply);
                    }
                }
            }

            if (depth == 0)
            {
                // Search for captures
                return Quiescence(board, alpha, beta);
            }

            var incheck = board.IsMyKingAttacked(board.CurrentTurn);

            // Check extension
            if (incheck)
            {
                depth++;
            }
            else
            {
                var eval = board.Evaluate();

                // Razoring
                if (depth < 2 && eval + 400 <= alpha)
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

                    board.MakeNullMove();
                    Ply++;
                    var score = -Negamax(board, -beta, -beta + 1,
                        (uint)Math.Max((int)depth - r, 0), false);
                    board.UndoNullMove();
                    Ply--;

                    if (_maxTime.HasValue && (ulong)_timer.ElapsedMilliseconds > _maxTime.Value)
                    {
                        return score;
                    }

                    if (score >= beta)
                    {
                        return score;
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

                // Pawn drop mate?
                if (move.GetDrop() == 1
                    && (
                        move.GetPieceType() == (uint)Piece.SentePawn ||
                        move.GetPieceType() == (uint)Piece.GotePawn)
                    && board.IsMyKingAttacked(board.CurrentTurn))
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
                                                 && PrincipalVariationTable[0][Ply] != move
                                                 && !incheck
                                                 && !board.IsMyKingAttacked(board.CurrentTurn.Invert()))
                    {
                        var dp = legals >= MoreReductionDepthLimit ? depth / 2 : depth - 2;
                        score = -Negamax(board, -alpha - 1, -alpha, dp);
                        if (_maxTime.HasValue && (ulong)_timer.ElapsedMilliseconds > _maxTime.Value)
                        {
                            board.UndoMove(move);
                            Ply--;
                            return score;
                        }
                    }
                    else
                    {
                        score = alpha + 1;
                    }

                    if (score > alpha)
                    {
                        score = -Negamax(board, -alpha - 1, -alpha, depth - 1);
                        if (_maxTime.HasValue && (ulong)_timer.ElapsedMilliseconds > _maxTime.Value)
                        {
                            board.UndoMove(move);
                            Ply--;
                            return score;
                        }

                        // Prove this position is good...
                        if (score > alpha && score < beta)
                        {
                            score = -Negamax(board, -beta, -alpha, depth - 1);
                            if (_maxTime.HasValue && (ulong)_timer.ElapsedMilliseconds > _maxTime.Value)
                            {
                                board.UndoMove(move);
                                Ply--;
                                return score;
                            }
                        }
                    }
                }
                else
                {
                    score = -Negamax(board, -beta, -alpha, depth - 1);
                    if (_maxTime.HasValue && (ulong)_timer.ElapsedMilliseconds > _maxTime.Value)
                    {
                        board.UndoMove(move);
                        Ply--;
                        return score;
                    }
                }

                board.UndoMove(move);
                Ply--;

#if DEBUG
                Debug.Assert(oHash == board.Hash, "Hash not equal");
#endif

                // fail-hard beta cutoff
                if (score >= beta)
                {
                    if (move.GetCapture() == 0 && move.GetDrop() == 0)
                    {
                        // Killer Moves
                        KillerMoves[1][Ply] = KillerMoves[0][Ply];
                        KillerMoves[0][Ply] = move;
                    }

                    // fails high
                    return beta;
                }

                // better move
                if (score > alpha)
                {
                    if (move.GetCapture() == 0 && move.GetDrop() == 0)
                    {
                        // History moves
                        HistoryMoves[move.GetPieceType()][move.GetTarget()] += (int)depth;
                    }

                    // PV
                    alpha = score;

                    // PVS Enable
                    foundPv = true;

                    PrincipalVariationTable[Ply][Ply] = move;
                    for (var next = Ply + 1; next < PrincipalVariationLengths[Ply + 1]; next++)
                    {
                        PrincipalVariationTable[Ply][next] = PrincipalVariationTable[Ply + 1][next];
                    }

                    PrincipalVariationLengths[Ply] = PrincipalVariationLengths[Ply + 1];
                }

                if (Math.Abs(score - Checkmate) < 100)
                {
                    return score;
                }
            }

            if (legals == 0)
            {
                return (int)Ply - Checkmate;
            }

            if (entry.Flags == TranspositionFlag.Invalid || alpha != originalAlpha)
            {
                if (entry.Depth <= depth)
                {
                    var entryType = alpha <= originalAlpha
                        ? TranspositionFlag.Alpha
                        : alpha >= beta
                            ? TranspositionFlag.Beta
                            : TranspositionFlag.Exact;

                    var valueToSave = TranspositionTable.TranspositionTable.RegularToTtScore(alpha, (int)Ply);

                    TranspositionTable.TranspositionTable.Add(board.Hash,
                        new TranspositionEntry(board.Hash, (byte)depth, entryType, valueToSave));
                }
            }

            return alpha;
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public int Quiescence(Board.Board board, int alpha, int beta)
        {
            // Nodes++;

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

                if (_maxTime.HasValue && (ulong)_timer.ElapsedMilliseconds > _maxTime.Value)
                {
                    return score;
                }

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