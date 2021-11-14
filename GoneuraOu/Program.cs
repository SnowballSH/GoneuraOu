using System;
using GoneuraOu.Bitboard;
using GoneuraOu.Board;
using GoneuraOu.Logic;

Board board = new();
board.PrintBoard();

board.LoadSFen("3bk/P4/2+r2/G2P1/KB3[SSrg] b - 34");
board.PrintBoard();

board.GenerateAllMoves();
