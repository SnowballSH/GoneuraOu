using System;
using System.Linq;
using GoneuraOu.Bitboard;
using GoneuraOu.Board;
using GoneuraOu.Common;
using GoneuraOu.Logic;

// Board board = new();
Board board = new("3bk/P4/2+r2/G2P1/KB3[SSrg] b - 34");

board.PrintBoard();

var ml = board.GenerateAllMoves();

Console.WriteLine(ml.Count);

foreach (var move in ml.Where(move => move.GetCapture() != 0))
{
    Console.ReadLine(); // pause

    Console.WriteLine(move.ToUsi());
    var nb = board.MakeMove(move);
    nb.PrintBoard();
    nb.Occupancies[0].BitboardPrint();
    nb.Occupancies[1].BitboardPrint();
    nb.Occupancies[2].BitboardPrint();

    Console.ReadLine(); // pause

    board.PrintBoard();
}