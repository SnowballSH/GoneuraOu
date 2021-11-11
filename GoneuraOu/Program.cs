using System;
using GoneuraOu.Bitboard;
using GoneuraOu.Board;
using GoneuraOu.Logic;

var bb = 0u.SetBitAt(Square.B3);

bb.BitboardPrint();
Console.WriteLine(Constants.SquareCoords[bb.Lsb1()]);
0u.SetBitAt(bb.Lsb1()).BitboardPrint();

/*
for (var square = 0; square < Constants.BoardArea; square++)
{
    Console.WriteLine(
        $"{Constants.Alphabets[square % Constants.BoardSize]}{Constants.BoardSize - square / Constants.BoardSize}");
    Attacks.GoldAttacks[square].BitboardPrint();
}
*/