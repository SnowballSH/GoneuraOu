using GoneuraOu.Common;
using GoneuraOu.Evaluation;
using GoneuraOu.Logic;

namespace GoneuraOu.Search
{
    public class Searcher
    {
        public int Nodes;
        public int Ply;
        public uint FinalBestMove;

        public int Negamax(Board.Board board, int alpha, int beta, uint depth)
        {
            if (depth == 0)
            {
                return Quiescence(board, alpha, beta);
            }

            Nodes++;

            var bestMove = 0u;

            var moves = board.GeneratePseudoLegalMoves();
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
            var evaluation = ClassicalEvaluation.Evaluate(board);

            if (evaluation >= beta)
            {
                return beta;
            }

            if (evaluation > alpha)
            {
                alpha = evaluation;
            }

            var moves = board.GenerateCaptureMoves();
            foreach (var move in moves)
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