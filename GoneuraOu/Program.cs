using System;
using GoneuraOu.Bitboard;
using GoneuraOu.Board;
using GoneuraOu.Logic;

for (var square = 0; square < Constants.BoardArea; square++)
{
    Console.WriteLine(
        $"{Constants.Alphabets[square % Constants.BoardSize]}{Constants.BoardSize - square / Constants.BoardSize}");
    Attacks.GoldAttacks[square].BitboardPrint();
}