using System;
using System.Linq;
using GoneuraOu.Bitboard;
using GoneuraOu.Board;
using GoneuraOu.Commands;
using GoneuraOu.Common;
using GoneuraOu.Evaluation;
using GoneuraOu.Logic;

void ProtocolMain()
{
    var proto = new Protocol();

    proto.StartProtocol();
}

Board board = new();
board.PrintBoard();
Console.WriteLine(board.Evaluate());
