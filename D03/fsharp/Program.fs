open System
open System.Diagnostics
open System.IO
open System.Text.RegularExpressions

let getInputAsync filePath = File.ReadAllTextAsync(filePath)
                             |> Async.AwaitTask

let multiplicationSumRegex input =
    let pattern = @"mul\((\d+),(\d+)\)|do\(\)|don't\(\)"
    let mutable take = true
    Regex.Matches(input, pattern)
    |> Seq.map (fun m ->
        match m.Groups[0].Value with
        | v when v = "do()"          -> take <- true; 0
        | v when v = "don't()"       -> take <- false; 0
        | _ when not take            -> 0
        | v when v.StartsWith("mul") -> (*) (m.Groups[1].Value |> int) (m.Groups[2].Value |> int)
        | _                          -> 0)
    |> Seq.sum


let inline (<<) a b = 10 * a + int b - int '0'

let rec multiplicationSumManual (input: ReadOnlySpan<char>) =
    match input with
    | i when i.StartsWith("mul(") -> multiplicationLeftPart (i.Slice "mul(".Length) 0
    | i when i.StartsWith("don't()") -> multiplicationIgnore (i.Slice "don't()".Length)
    | i -> multiplicationSumManual (i.Slice 1)
    
// try to load left part, and if valid load right part
and multiplicationLeftPart (input: ReadOnlySpan<char>) a =
    match input.Length with
    | 0 | 1 | 2 -> 0
    | _ when Char.IsAsciiDigit input[0] -> multiplicationLeftPart (input.Slice 1) (a << input[0])
    | _ when input[0] = ',' -> multiplicationRightPart (input.Slice 1) a 0
    | _ -> multiplicationSumManual input

// try to load right part, and if valid multiply
and multiplicationRightPart (input: ReadOnlySpan<char>) a b =
    match input.Length with
    | 0 -> 0
    | _ when Char.IsAsciiDigit input[0] -> multiplicationRightPart (input.Slice 1) a (b << input[0])
    | _ when input[0] = ')' -> a * b + multiplicationSumManual (input.Slice 1)
    | _ -> multiplicationSumManual input
        
// ignore until do()
and multiplicationIgnore (input: ReadOnlySpan<char>) =
    match input.IndexOf("do()") with
    | -1 -> 0
    | i -> multiplicationSumManual (input.Slice (i + "do()".Length))

[<EntryPoint>]
let main _ =
    let input = getInputAsync "input.txt" |> Async.RunSynchronously
    
    let timestamp = Stopwatch.GetTimestamp()
    let multiplied = multiplicationSumRegex input
    let elapsed = Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds
    printfn $"Multiplication result: %d{multiplied}"
    printfn $"Elapsed time: %f{elapsed} ms"
    printfn ""
    
    let timestamp = Stopwatch.GetTimestamp()
    let multiplied = multiplicationSumManual (input.AsSpan())
    let elapsed = Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds
    printfn $"Multiplication result: %d{multiplied}"
    printfn $"Elapsed time: %f{elapsed} ms"
    printfn ""
    
    0 // return an integer exit code