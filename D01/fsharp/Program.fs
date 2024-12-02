// For more information see https://aka.ms/fsharp-console-apps

open System
open System.Diagnostics
open System.IO
open System.Collections.Generic

let readFile (filePath: string) =
    // open file and read all lines
    let lines = File.ReadLines(filePath) |> Seq.toArray 
    
    let x: int[] = Array.zeroCreate lines.Length
    let y: int[] = Array.zeroCreate lines.Length
    
    // load values
    lines
    |> Array.iteri (fun index line ->
        let parts = line.Split("   ");
        x[index] <- int parts[0]
        y[index] <- int parts[1]
    )
    
    Array.sortInPlace x
    Array.sortInPlace y
    
    // return the two queues
    x, y
    
let addOrIncrement key (dict: Dictionary<int, int>) =
    match dict.TryGetValue(key) with
    | true, value -> dict[key] <- value + 1
    | false, _ -> dict.Add(key, 1)
    
let rec calculateDistance index (a: int[]) (b: int[]) =
    match index with
    | _ when index >= min a.Length b.Length -> 0
    | _ -> abs(a[index] - b[index]) + calculateDistance (index + 1) a b
    
let calculateSimilarityScore (a: int[]) (b: int[]) =
    let occurenceMap = Dictionary<int, int>(b.Length)
    Array.iter (fun key -> addOrIncrement key occurenceMap) b
    Array.fold (fun acc key -> acc + key * (match occurenceMap.TryGetValue(key) with
                                           | true, value -> value
                                           | _ -> 0)) 0 a
    
let timestamp = Stopwatch.GetTimestamp()
let leftInput, rightInput = readFile "Input.txt"
let totalDistance = calculateDistance 0 leftInput rightInput
let similarityScore = calculateSimilarityScore leftInput rightInput
let elapsedMilliseconds = Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds

printfn $"The total distance is: {totalDistance}"
printfn $"The similarity score is: {similarityScore}"
printfn $"Elapsed time: {elapsedMilliseconds}ms"

