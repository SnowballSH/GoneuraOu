using System;
using GoneuraOu.Board;

var board = new Board();
board.PrintBoard();

board.LoadSFen("3bk/4p/2+r2/G2P1/KB3[SSrg] w - 34");
board.PrintBoard();