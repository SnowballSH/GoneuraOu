﻿namespace GoneuraOu.Search
{
    public static class MoveOrdering
    {
        //     P   G   S   R   B   K   +P  +S  +R  +B
        // P   101 301 201 501 401 901 301 301 701 601
        // G   103 303 203 503 403 903 303 303 703 603
        // S   102 302 202 502 402 902 302 302 702 602
        // R   105 305 205 505 405 905 305 305 705 605
        // B   106 306 206 506 406 906 306 306 706 606
        // K   109 309 209 509 409 909 309 309 709 609
        // +P  103 303 203 503 403 903 303 303 703 603
        // +S  103 303 203 503 403 903 303 303 703 603
        // +R  107 307 207 507 407 907 307 307 707 607
        // +B  106 306 206 506 406 906 306 306 706 606
        public static readonly int[,] MvvLvaTable =
        {
            {
                101, 301, 201, 501, 401, 901, 301, 301, 701, 601,
                101, 301, 201, 501, 401, 901, 301, 301, 701, 601
            },
            {
                103, 303, 203, 503, 403, 903, 303, 303, 703, 603,
                103, 303, 203, 503, 403, 903, 303, 303, 703, 603
            },
            {
                102, 302, 202, 502, 402, 902, 302, 302, 702, 602,
                102, 302, 202, 502, 402, 902, 302, 302, 702, 602
            },
            {
                105, 305, 205, 505, 405, 905, 305, 305, 705, 605,
                105, 305, 205, 505, 405, 905, 305, 305, 705, 605
            },
            {
                106, 306, 206, 506, 406, 906, 306, 306, 706, 606,
                106, 306, 206, 506, 406, 906, 306, 306, 706, 606
            },
            {
                109, 309, 209, 509, 409, 909, 309, 309, 709, 609,
                109, 309, 209, 509, 409, 909, 309, 309, 709, 609
            },
            {
                103, 303, 203, 503, 403, 903, 303, 303, 703, 603,
                103, 303, 203, 503, 403, 903, 303, 303, 703, 603
            },
            {
                103, 303, 203, 503, 403, 903, 303, 303, 703, 603,
                103, 303, 203, 503, 403, 903, 303, 303, 703, 603
            },
            {
                107, 307, 207, 507, 407, 907, 307, 307, 707, 607,
                107, 307, 207, 507, 407, 907, 307, 307, 707, 607
            },
            {
                106, 306, 206, 506, 406, 906, 306, 306, 706, 606,
                106, 306, 206, 506, 406, 906, 306, 306, 706, 606
            },
            {
                101, 301, 201, 501, 401, 901, 301, 301, 701, 601,
                101, 301, 201, 501, 401, 901, 301, 301, 701, 601
            },
            {
                103, 303, 203, 503, 403, 903, 303, 303, 703, 603,
                103, 303, 203, 503, 403, 903, 303, 303, 703, 603
            },
            {
                102, 302, 202, 502, 402, 902, 302, 302, 702, 602,
                102, 302, 202, 502, 402, 902, 302, 302, 702, 602
            },
            {
                105, 305, 205, 505, 405, 905, 305, 305, 705, 605,
                105, 305, 205, 505, 405, 905, 305, 305, 705, 605
            },
            {
                106, 306, 206, 506, 406, 906, 306, 306, 706, 606,
                106, 306, 206, 506, 406, 906, 306, 306, 706, 606
            },
            {
                109, 309, 209, 509, 409, 909, 309, 309, 709, 609,
                109, 309, 209, 509, 409, 909, 309, 309, 709, 609
            },
            {
                103, 303, 203, 503, 403, 903, 303, 303, 703, 603,
                103, 303, 203, 503, 403, 903, 303, 303, 703, 603
            },
            {
                103, 303, 203, 503, 403, 903, 303, 303, 703, 603,
                103, 303, 203, 503, 403, 903, 303, 303, 703, 603
            },
            {
                107, 307, 207, 507, 407, 907, 307, 307, 707, 607,
                107, 307, 207, 507, 407, 907, 307, 307, 707, 607
            },
            {
                106, 306, 206, 506, 406, 906, 306, 306, 706, 606,
                106, 306, 206, 506, 406, 906, 306, 306, 706, 606
            }
        };
    }
}