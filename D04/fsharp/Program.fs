// For more information see https://aka.ms/fsharp-console-apps

open System
open System.Diagnostics

let getInputText (filename: string) =
    #if DEBUG
    let sample = [|
        "MMMSXXMASM";
        "MSAMXMSMSA";
        "AMXSXMAAMM";
        "MSAMASMSMX";
        "XMASAMXAMM";
        "XXAMMXXAMA";
        "SMSMSASXSS";
        "SAXAMASAAA";
        "MAMMMXMMMM";
        "MXMXAXMASX";
    |]
    seq {
        for line in sample do
            yield line
    }
    #else
    seq {
        
        use reader = System.IO.File.OpenText(filename)
        while not reader.EndOfStream do
            yield reader.ReadLine()
    }
    #endif
 
type InputText(text: array<array<char>>) =
    let x = text.Length
    let y = Array.maxBy Array.length text |> Array.length
    
    member this.Text = text
    member this.X = x
    member this.Y = y
    
    static member Construct (lines: seq<string>) =
        let text = lines
                |> Seq.map (_.ToCharArray())
                |> Seq.toArray
        InputText(text)
        
type Direction =
    | Up
    | Down
    | Left
    | Right
    | UpLeft
    | UpRight
    | DownLeft
    | DownRight
    
    static member getTuple direction =
        match direction with
        | Up -> (-1, 0)
        | Down -> (1, 0)
        | Left -> (0, -1)
        | Right -> (0, 1)
        | UpLeft -> (-1, -1)
        | UpRight -> (-1, 1)
        | DownLeft -> (1, -1)
        | DownRight -> (1, 1)

let inline (++) (x1, y1) (x2, y2) = (x1 + x2, y1 + y2)

let isCorrectChar (input: InputText) (x: int, y: int) (required: char) =
    x >= 0 && x < input.X && y >= 0 && y < input.Y && input.Text[x].[y] = required
 
let isWord (input: InputText) (coordinates: int * int) (word: string) (direction: Direction) =
    let mutable coordinates = coordinates
    let mutable word = word.AsSpan()
    let mutable result = true
    
    while not word.IsEmpty && result = true do
        if not (isCorrectChar input coordinates word[0]) then
            result <- false
        else
            word <- word.Slice(1)
            coordinates <- coordinates ++ Direction.getTuple direction
        
    result

let countXmasOccurrences (input: InputText) =
    input.Text
    |> Array.mapi (fun x row ->
        row
        |> Array.mapi (fun y _ ->
            (isWord input (x, y) "XMAS" Up |> Convert.ToInt32)
            + (isWord input (x, y) "XMAS" Down |> Convert.ToInt32)
            + (isWord input (x, y) "XMAS" Left |> Convert.ToInt32)
            + (isWord input (x, y) "XMAS" Right |> Convert.ToInt32)
            + (isWord input (x, y) "XMAS" UpLeft |> Convert.ToInt32)
            + (isWord input (x, y) "XMAS" UpRight |> Convert.ToInt32)
            + (isWord input (x, y) "XMAS" DownLeft |> Convert.ToInt32)
            + (isWord input (x, y) "XMAS" DownRight |> Convert.ToInt32))
        |> Array.sum)
    |> Array.sum

let countCrossOccurrences (input: InputText) =
    input.Text
    |> Array.mapi (fun x row ->
        row
        |> Array.mapi (fun y _ ->
            (isWord input (x, y) "MAS" DownRight && isWord input (x + 2, y) "MAS" UpRight)
            || (isWord input (x, y) "MAS" DownRight && isWord input (x + 2, y) "SAM" UpRight)
            || (isWord input (x, y) "SAM" DownRight && isWord input (x + 2, y) "MAS" UpRight)
            || (isWord input (x, y) "SAM" DownRight && isWord input (x + 2, y) "SAM" UpRight))
        |> Array.map Convert.ToInt32
        |> Array.sum
    )
    |> Array.sum
    
[<EntryPoint>]
let main _ =
    let input = getInputText "input.txt"
    let inputText = InputText.Construct input
    
    let timestamp = Stopwatch.GetTimestamp()
    let occurrences = countXmasOccurrences inputText
    let elapsedMilliseconds = Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds
    printfn $"XMAS occurrences: %d{occurrences}"
    printfn $"Elapsed time: %f{elapsedMilliseconds} ms"
    
    let timestamp = Stopwatch.GetTimestamp()
    let occurrences = countCrossOccurrences inputText
    let elapsedMilliseconds = Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds
    printfn $"X-MAS occurrences: %d{occurrences}"
    printfn $"Elapsed time: %f{elapsedMilliseconds} ms"
    
    0