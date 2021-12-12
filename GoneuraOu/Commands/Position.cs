using System;
using System.Diagnostics;
using System.Linq;
using GoneuraOu.Common;

namespace GoneuraOu.Commands
{
    public static class Position
    {
        public static void DoPosition(this Protocol proto, string[] tokens)
        {
            Debug.Assert(tokens[0] == "position");
            var index = 1;
            if (index == tokens.Length) return;
            switch (tokens[index++])
            {
                case null:
                    return;
                case "startpos":
                    proto.CurrentPosition = new Board.Board();
                    break;
                case "fen":
                case "sfen":
                    string fen = tokens[index++];
                    while (index < tokens.Length && tokens[index] != "moves")
                    {
                        fen += " " + tokens[index++];
                    }

                    proto.CurrentPosition.LoadSFen(fen);
                    break;
                default:
                    // invalid
                    Console.WriteLine("Unknown subcommand after `position`");
                    break;
            }

            if (index == tokens.Length) return;
            if (tokens[index++] != "moves") return;

            var seq = tokens.Skip(index);
            foreach (var m in seq)
            {
                var move = m.UciToMove(proto.CurrentPosition);
                proto.CurrentPosition.MakeMoveUnchecked(move);
                // Assume GUI is mistake-free
            }
        }
    }
}