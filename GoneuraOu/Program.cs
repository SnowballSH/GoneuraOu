﻿using System;
using GoneuraOu.Board;
using GoneuraOu.Common;
using GoneuraOu.Logic;

// Board board = new();
Board board = new("3bk/P4/2+r2/G2P1/KB3[SSrg] b - 34");

board.PrintBoard();

var ml = board.GenerateAllMoves();

Console.WriteLine(ml.Count);

foreach (var move in ml)
{
    Console.ReadLine();  // pause

    Console.WriteLine(move.ToUsi());
    var nb = board.ShallowCopy();
    nb.MakeMove(move);
    nb.PrintBoard();

    Console.ReadLine();  // pause

    board.PrintBoard();
}