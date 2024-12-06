open System
open System.Collections.Generic
open System.Diagnostics

let getOrderMap (input: ReadOnlySpan<char>) =
    let map = Dictionary<int, HashSet<int>>()
    let mutable input = input
    
    while not input.IsEmpty && input.IndexOf("|") > 0 do
        let line = input.Slice(0, input.IndexOf(Environment.NewLine))
        let splitIndex = line.IndexOf("|")
        let value = Int32.Parse (line.Slice(0, splitIndex))
        let key = Int32.Parse (line.Slice(splitIndex + 1))
        
        let isSuccess, before = map.TryGetValue(key)
        if isSuccess then
            before.Add(value) |> ignore
        else
            map.Add(key, HashSet<int>[value])
        
        input <- input.Slice(line.Length + Environment.NewLine.Length)
        
    while input.StartsWith(Environment.NewLine) do input <- input.Slice(Environment.NewLine.Length)
    
    map, input.Length

let getInputAsNumbers (input: ReadOnlySpan<char>) =
    let numbers = List<int list>()
    let mutable input = input
    
    while not input.IsEmpty do
        // get one line 
        let newLineIndex = input.IndexOf(Environment.NewLine)
        let mutable line = input
        if newLineIndex > 0 then
            line <- input.Slice(0, newLineIndex)
            input <- input.Slice(line.Length + Environment.NewLine.Length)
        else
            line <- input
            input <- input.Slice(line.Length)
        
        // load numbers from line 
        let newNumbers = List<int>()
        while not line.IsEmpty do
            let separatorIndex = line.IndexOf(",")
            let number = if separatorIndex > 0 then line.Slice(0, separatorIndex) else line
            line <- line.Slice(number.Length + (if separatorIndex > 0 then 1 else 0))
            let numberInt = if number.IsEmpty then Int32.MinValue else Int32.Parse number
            newNumbers.Add(numberInt)
        numbers.Add(newNumbers |> Seq.toList)
        
    numbers

let rec isOrdered (map: Dictionary<int, HashSet<int>>) (numbers: int list) =
    match numbers with
    | x :: tail when map.ContainsKey(x) && tail |> List.exists (fun y -> map[x].Contains(y)) -> false
    | x :: tail -> isOrdered map tail
    | _ -> true
    
let getMiddleSumOfCorrectlyOrdered (map: Dictionary<int, HashSet<int>>) (numbers: List<int list>) =
    numbers
    |> Seq.filter (isOrdered map)
    |> Seq.map (fun x -> x[x.Length / 2])
    |> Seq.sum

let rec shouldSwap (map: Dictionary<int, HashSet<int>>) (x: int) (y: int) =
    match x, y with
    | x, y when map.ContainsKey(y) && map[y].Contains(x) -> 1
    | x, y when x = y -> 0
    | _ -> -1
    
let getMiddleSumOfIncorrectlyOrdered (map: Dictionary<int, HashSet<int>>) (numbers: List<int list>) =
    numbers
    |> Seq.filter (fun x -> not (isOrdered map x))
    |> Seq.map (fun x -> x |> List.sortWith (shouldSwap map))
    |> Seq.map (fun x -> x[x.Length / 2])
    |> Seq.sum
    
[<EntryPoint>]
let main _ =
    let input = System.IO.File.ReadAllText "input.txt"
    
    let inputSpan = input.AsSpan()
    let map, sizeOffset = getOrderMap inputSpan
    let numbers = getInputAsNumbers (inputSpan.Slice(inputSpan.Length - sizeOffset))
    
    let timestamp = Stopwatch.GetTimestamp()
   
    let middleSum = getMiddleSumOfCorrectlyOrdered map numbers
    let middleSumOfIncorrect = getMiddleSumOfIncorrectlyOrdered map numbers
   
    let elapsed = Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds
    
    printfn $"Middle sum of correctly ordered pages: %d{middleSum}"
    printfn $"Middle sum of incorrectly ordered pages: %d{middleSumOfIncorrect}"
    printfn $"Elapsed time: %f{elapsed} ms"
    
    0 // return an integer exit code