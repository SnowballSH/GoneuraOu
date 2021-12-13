namespace GoneuraOu.Board
{
    public static class NeuralNetAdapt
    {
        public static bool[] ToNeuralNetFormat(this Board board)
        {
            var activated = new bool[25 * 20 + 2 * 20];
            for (var sq = 0; sq < 25; sq++)
            {
                for (var pt = 0; pt < 20; pt++)
                {
                    activated[sq * 20 + pt] = board.PieceLoc[sq] == pt;
                }
            }

            for (var i = 0; i < 20; i++)
            {
                activated[25 * 20 + i] = board.Pocket[i / 10, i % 10];
            }

            return activated;
        }
    }
}