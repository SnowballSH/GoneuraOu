# GoneuraOu

UCI Minishogi (5x5 shogi) Engine

Current Strength: Around Fairy-Stockfish level 5-6

Defeats Fairy-Stockfish level 4 in 11 moves:

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

Defeats Fairy-Stockfish level 3 in 24 moves:

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
