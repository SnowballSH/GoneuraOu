module Tests

open System
open GoneuraOu.Commands
open Xunit
open GoneuraOu

[<Fact>]
let ``Perft 3`` () =
    let expected = 2512
    let actual = Perft.PerftInternal(Board.Board(), 3u)
    Assert.Equal(expected, actual)

[<Fact>]
let ``Perft 5`` () =
    let expected = 533203
    let actual = Perft.PerftInternal(Board.Board(), 5u)
    Assert.Equal(expected, actual)
