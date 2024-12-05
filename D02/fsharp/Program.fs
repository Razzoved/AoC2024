// For more information see https://aka.ms/fsharp-console-apps

open System.Diagnostics
open System.IO

let getLines filePath =
    File.ReadLines filePath
    |> Seq.filter (fun line -> line <> "" && not (line.StartsWith '#'))
    |> Seq.map (fun line -> line.Split ' ' |> Array.toList |> List.map int)

let isInOrder a b = a < b && b - 3 <= a

let rec isAscending numbers =
    match numbers with
    | x :: y :: tail when isInOrder x y -> isAscending (y :: tail)
    | _ :: _ :: _ -> false
    | _ -> true

let rec isDescending numbers =
    match numbers with
    | x :: y :: tail when isInOrder y x -> isDescending (y :: tail)
    | _ :: _ :: _ -> false
    | _ -> true

let rec isAscendingDampened numbers =
    match numbers with
    | x :: y :: z :: tail when not (isInOrder y z) -> isAscending (x :: z :: tail) || isAscending (x :: y :: tail)
    | x :: y :: tail when not (isInOrder x y) -> isAscending (y :: tail)
    | _ :: y :: tail -> isAscendingDampened (y :: tail)
    | _ -> true

let rec isDescendingDampened numbers =
    match numbers with
    | x :: y :: z :: tail when not (isInOrder z y) -> isDescending (x :: z :: tail) || isDescending (x :: y :: tail)
    | x :: y :: tail when not (isInOrder y x) -> isDescending (y :: tail)
    | _ :: y :: tail -> isDescendingDampened (y :: tail)
    | _ -> true

[<EntryPoint>]
let main _ =
    let timestamp = Stopwatch.GetTimestamp()

    let lines = getLines "input.txt"

    let safeCount =
        lines
        |> Seq.filter (fun line -> isAscending line || isDescending line)
        |> Seq.length

    let dampenedCount =
        lines
        |> Seq.filter (fun line -> isAscendingDampened line || isDescendingDampened line)
        |> Seq.length

    let elapsed = Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds

    printfn $"Safe records: {safeCount}"
    printfn $"Damp records: {dampenedCount}"
    printfn $"Elapsed: {elapsed} ms"

    0 // return an integer exit code
