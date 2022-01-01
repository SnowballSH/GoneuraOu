# GoneuraOu

Decent UCI Minishogi (5x5 shogi) Engine

Current Strength: Around **1900 ELO**

## Technology

Technologies used to build this engine: (crossed-out means in the future)

- Move Generation
    - Bitboards
    - Pre-generated pattern tables
    - Modified Kindergarten Bitboards for Slider attacks
    - Incremental Pawn Locations
    - Pawn drop mate detection

- Search
    - Negamax Search
    - Quiescence Search
    - Iterative Deepening
    - PVS Search
    - Null-Move Pruning
    - Check Extension
    - Razoring
    - Deep Razoring
    - Late-Move-Reduction (LMR)
    - Transposition Table (a little buggy though...)
    - ~~SEE (Static Exchange Evaluation)~~
    - ~~MTD(f)~~
    - ~~Futility Pruning~~

- Move Ordering
    - MVV-LVA
    - History Moves
    - Killer Moves
    - Second Killer Moves
    - ~~Checks~~

- Evaluation
    - Piece Values
    - Piece-square Table
    - King-Distance evaluation (inspired from Computer Shogi papers)
    - King Safety
    - Slider activity
    - King activity
    - ~~Pins~~
    - ~~NNUE for accurate evaluation~~

## Games

Games during development progress:

Finds forced checkmate in 14 (halfmoves) against Fairy-Stockfish 1800 ELO: (blitz 3+2)

```
[White "GoneuraOu"]
[Black "Fairy-Stockfish 1800 ELO"]
[Result "1-0"]
[TimeControl "40/180+2"]
[Variant "minishogi"]

1. Sb2 Bd3 2. Gc2 Gd4 3. Sc3 Bxc2
4. Sxd4 Sxd4 5. Bxc2 S@b2 6. Kxb2 G@c3
7. Kc1 Rb5 8. Rxe4 Kd5 9. B@b3 Rxb3
10. Bxb3 B@c4 11. S@e3 Gd2 12. Kb1 Gc1
13. Kxc1 Sc5 14. Bxc4 Sxc4 15. G@d4 { Checkmate } 1-0
```

Slowly improves position to +4 and defeats Fairy-Stockfish level 5 in 16 moves: (blitz 3+2)

```
[White "Fairy-Stockfish Level 5"]
[Black "GoneuraOu"]
[Result "0-1"]
[TimeControl "30/180+2"]
[Variant "minishogi"]

1. Sd2 Sd4 2. Bc2 Bc4 3. Rc1 Rb5
4. Rd1 Bb3 5. Sc1 Bxc2 6. Gxc2 B@a4
7. B@b2 Rxb2 8. Kxb2 B@c3 9. Kb1 Bxc2
10. Kxc2 Ba1+ 11. B@b4 +Bxa2 12. R@a3 +Bxa3
13. Rxd4 G@b3 14. Kd2 +Bxc1 15. Ke1 S@e2
16. Kxe2 R@e3 { Checkmate } 0-1
```

Defeats Fairy-Stockfish level 4 in 11 moves: (blitz 3+2)

```
[White "Fairy-Stockfish Level 4"]
[Black "GoneuraOu"]
[Result "0-1"]
[TimeControl "40/180+2"]
[Variant "minishogi"]

1. Sc2 Sd4 2. Gb2 Gc5 3. Sc3 Sxc3
4. Gxc3 S@d2 5. S@b4 Sxe1+ 6. Bb3 Gxb4
7. Gxb4 Bd3 8. G@b1 S@c3 9. Gc1 R@b1
10. Gxb1 Bxb1+ 11. Kxb1 G@b2 { Checkmate } 0-1
```

Defeats Fairy-Stockfish level 3 in 24 moves: (bullet 0.5+2)

```
[White "GoneuraOu"]
[Black "Fairy-Stockfish Level 3"]
[Result "1-0"]
[TimeControl "40/30+2"]
[Variant "minishogi"]

1. Sb2  Bd3  2. Gc2 Bc4  3. Gb3  Bd3 
4. Ga4  Rxa4  5. Bxa4 G@b4  6. R@a5  Sd4 
7. Rc1  Pe3  8. Bd1 Ke4  9. Sb3  Gb5 
10. Ra3+  Gbc5  11. +Rb2 Gcc4  12. Sc2  Ke5 
13. Sxd3  Gxd3  14. B@a5 S@c3  15. Rxc3  Gxc3 
16. +Rxc3  R@b5  17. Bb4+ Rxb4  18. +Rxb4  B@d2 
19. +Rb5  Be1+  20. G@c4 +Bc3  21. Gxc3  Pe2 
22. +Rxd5  Kxd5  23. G@c4 Ke5  24. G4xd4 { Checkmate } 1-0
```
