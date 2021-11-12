using System;
using GoneuraOu.Bitboard;
using GoneuraOu.Board;
using GoneuraOu.Common;
using GoneuraOu.Logic;

var occupancy = 0u.SetBitAt(Square.B3).SetBitAt(Square.C1).SetBitAt(Square.D2).SetBitAt(Square.E4);

occupancy.BitboardPrint();

Attacks.GetRookAttacks((int) Square.B2, occupancy).BitboardPrint();