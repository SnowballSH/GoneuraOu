# GoneuraOu

UCI Minishogi (5x5 shogi) Engine

Current Strength: Around Fairy-Stockfish level 6-7

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

Intense game with itself, Dec 30 2021 (blitz 3+2)

```
[White "GoneuraOu"]
[Black "GoneuraOu"]
[Result "1-0"]
[TimeControl "40/180+2"]
[Variant "minishogi"]

1. Sb2 {+0.02/12 11s} Sd4 {-0.02/11 11s} 2. Gc2 {+0.09/11 11s}
Bc4 {+0.03/10 11s} 3. Gb3 {+0.08/11 10s} Rb5 {-0.08/10 10s}
4. Gxc4 {+0.16/10 9.8s} Gxc4 {+0.34/9 9.8s} 5. Be2 {-0.34/8 9.4s}
G@c2 {+0.79/9 9.4s} 6. B@a3 {-0.29/8 9.0s} Sd3 {+1.66/9 9.0s}
7. Bxd3 {+0.33/11 8.7s} Gxd3 {+1.00/9 8.7s} 8. S@c4 {-1.00/8 8.3s}
B@d4 {+1.00/7 8.3s} 9. Sxb5+ {+0.47/9 8.0s} Gxb2 {-0.47/9 8.0s}
10. Bxb2 {+0.97/8 7.7s} Bxb2 {-0.97/8 7.7s} 11. Kxb2 {+0.97/7 7.4s}
B@c3 {-0.97/7 7.4s} 12. Kb3 {+1.26/8 7.2s} S@c2 {-1.26/8 7.2s}
13. Ka3 {+1.76/7 6.9s} Bxe1+ {-1.76/7 6.9s} 14. B@b2 {+1.76/6 6.7s}
R@c3 {-2.82/7 6.7s} 15. Bxc3 {+2.82/7 6.4s} Gxc3 {-2.82/7 6.4s}
16. R@c5 {+3.08/9 6.2s} Kd4 {-3.08/8 6.2s} 17. G@c4 {+3.08/8 6.0s}
Gxc4 {-3.53/9 6.0s} 18. R@d5 {+3.53/9 5.8s} Ke3 {-3.53/8 5.8s}
19. Rxc4+ {+3.53/8 5.6s} B@c1 {-3.53/7 5.6s} 20. Ka4 {+9.61/8 5.4s}
G@d3 {-9.61/8 5.4s} 21. Rxd3+ {+9.54/8 5.2s} Sxd3 {-9.30/9 5.2s}
22. +Rxc1 {+9.30/9 5.1s} R@d1 {-9.30/8 5.1s} 23. +Rb2 {+9.30/8 4.9s}
Rd2+ {-9.20/8 4.9s} 24. B@c5 {+8.92/8 4.8s} Ke2 {-9.26/9 4.8s}
25. +Ra3 {+9.26/9 4.6s} +Rc2 {-9.26/8 4.6s} 26. Bd4+ {+9.46/9 4.5s}
+Bd1 {-9.46/8 4.5s} 27. Ka5 {+9.42/8 4.4s} Pe3 {-9.48/8 4.4s}
28. +Bb2 {+9.98/7 4.3s} +Rd2 {-9.48/9 4.3s} 29. +Sb4 {+9.98/8 4.2s}
Sc2 {-10.47/9 4.2s} 30. +Bc3 {+10.45/9 4.0s} +Rxc3 {-10.30/9 4.0s}
31. +Rxc3 {+10.27/9 3.9s} B@d2 {-10.27/8 3.9s} 32. R@c5 {+11.20/9 3.8s}
Bxc3 {-11.20/8 3.8s} 33. Rxc3+ {+11.27/8 3.8s} R@d5 {-11.12/7 3.8s}
34. G@c5 {+10.86/7 3.7s} Rd2 {-10.51/8 3.7s} 35. B@a4 {+12.14/9 3.6s}
Ke1 {-12.15/9 3.6s} 36. +Rxe3 {+12.15/8 3.5s} +Be2 {-12.15/7 3.5s}
37. +Rxe2 {+12.65/7 3.4s} Kxe2 {-12.15/7 3.4s} 38. B@c3 {+12.15/7 3.4s}
R@d1 {-12.27/8 3.4s} 39. Bb5+ {+12.21/8 3.3s} Sd3 {-12.42/8 3.3s}
40. Bxd2 {+12.37/9 3.2s} Rxd2+ {-12.37/8 3.2s} 41. R@e5 {+12.79/8 12s}
Kd1 {-12.74/8 12s} 42. Rd5+ {+12.74/8 12s} +Rxa2 {-12.49/8 12s}
43. P@a4 {+12.49/7 11s} Sd2 {-12.78/9 11s} 44. +Bc4 {+13.28/8 11s}
+Rc2 {-13.28/7 11s} 45. +Bb3 {+13.34/8 10s} +Rxb3 {-13.34/7 10s}
46. +Sxb3 {+13.86/8 9.9s} P@c2 {-13.86/7 9.9s} 47. +Rxd2 {+34.30/10 9.5s}
Kxd2 {-22.72/8 9.5s} 48. R@d5 {+22.72/8 9.1s} B@d3 {-22.72/7 9.1s}
49. S@c3 {+9876.41/9 8.7s} Kc1 {-9876.42/9 8.7s} 50. Rxd3+ {+9875.93/9 8.4s}
R@a2 {-9875.94/8 8.4s} 51. +Rd2 {+9876.45/9 8.1s} Kb1 {-9876.46/8 8.1s}
52. G@b2 {+9876.47/8 7.8s} Ka1 {-9876.48/9 7.8s} 53. Gxa2 {+9876.49/9 7.5s}
Kxa2 {-9876.50/8 7.5s} 54. +Rxc2 {+9876.51/8 7.2s} Ka1 {-9876.52/8 7.2s}
55. R@a2 {+9876.53/8 7.0s, Sente mates} 1-0
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
