﻿using System;
using System.Linq;
using GoneuraOu.Bitboard;
using GoneuraOu.Board;
using GoneuraOu.Commands;
using GoneuraOu.Common;
using GoneuraOu.Logic;

Board board = new();
// Board board = new("3bk/P4/2+r2/G2P1/KB3[SSrg] b - 34");

// board = board.MakeMove(MoveEncode.EncodeMove((int)Square.S1E, (int)Square.S1C, (int)Piece.SenteRook, 0, 0, 0))!;

board.PrintBoard();

board.PerftRootPrint(3);