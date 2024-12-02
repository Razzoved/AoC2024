// See https://aka.ms/new-console-template for more information

using System.Diagnostics;


var timestamp = Stopwatch.GetTimestamp();

var numbers = GetNumbers("Input.txt");
var totalDistance = CalculateDistance(numbers.x, numbers.y);
var similarityScore = CalculateSimilarityScore(numbers.x, numbers.y);

var elapsedMilliseconds = Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds;

Console.WriteLine($"The total distance is: {totalDistance}");
Console.WriteLine($"The similarity score is: {similarityScore}");
Console.WriteLine($"Elapsed time: {elapsedMilliseconds}ms");

await Task.Delay(1000);

var timestampLinq = Stopwatch.GetTimestamp();

var numbersLinq = GetNumbersLinq("Input.txt");
var totalDistanceLinq = CalculateDistanceLinq(numbersLinq.x, numbersLinq.y);
var similarityScoreLinq = CalculateSimilarityScoreLinq(numbersLinq.x, numbersLinq.y);

var elapsedMillisecondsLinq = Stopwatch.GetElapsedTime(timestampLinq).TotalMilliseconds;

Console.WriteLine($"The total distance is linq: {totalDistanceLinq}");
Console.WriteLine($"The similarity score is linq: {similarityScoreLinq}");
Console.WriteLine($"Elapsed time linq: {elapsedMillisecondsLinq}ms");

return;


(int[] x, int[] y) GetNumbers(in string filePath)
{
    var lines = File.ReadLines(filePath).ToArray();

    var x = new int[lines.Length];
    var y = new int[lines.Length];

    for (int i = 0; i < lines.Length; i++)
    {
        var parts = lines[i].Split("   ");
        x[i] = int.Parse(parts[0]);
        y[i] = int.Parse(parts[1]);
    }

    Array.Sort(x);
    Array.Sort(y);

    return (x, y);
}


(IEnumerable<int> x, IEnumerable<int> y) GetNumbersLinq(in string filePath)
{
    var linq = File
        .ReadLines(filePath)
        .Select(line => line.Split("   "))
        .Select(parts => (int.Parse(parts[0]), int.Parse(parts[1])))
        .ToArray();
    return (linq.Select(x => x.Item1).Order(), linq.Select(x => x.Item2).Order());
}

int CalculateDistance(int[] a, int[] b)
{
    var sum = 0;
    for (int i = 0; i < Math.Min(a.Length, b.Length); i++) sum += Math.Abs(a[i] - b[i]);
    return sum;
}


int CalculateDistanceLinq(IEnumerable<int> a, IEnumerable<int> b)
    => a.Zip(b, (x, y) => Math.Abs(x - y)).Sum();

int CalculateSimilarityScore(int[] a, int[] b)
{
    var occurences = new Dictionary<int, int>(b.Length);
    foreach (var n in b)
    {
        if (occurences.TryGetValue(n, out var value))
            occurences[n] = ++value;
        else
            occurences[n] = 1;
    }

    var sum = 0;
    foreach (var n in a)
    {
        if (occurences.TryGetValue(n, out var value)) sum += n * value;
    }

    return sum;
}
    
int CalculateSimilarityScoreLinq(IEnumerable<int> a, IEnumerable<int> b)
{
    var occurences = b
        .GroupBy(x => x)
        .ToDictionary(x => x.Key, x => x.Count());
    return a.Sum(x => occurences.TryGetValue(x, out var value) ? x * value : 0);
}