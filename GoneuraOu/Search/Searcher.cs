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

        public Searcher()
        {
            KillerMoves = new uint[2, 128];
            HistoryMoves = new int[20, 25];
        }

        public int Negamax(Board.Board board, int alpha, int beta, uint depth)
        {
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
                    // fails high
                    return beta;
                }

                // better move
                if (score > alpha)
                {
                    alpha = score;

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