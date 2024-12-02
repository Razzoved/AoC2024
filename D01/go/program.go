package main

import (
    "fmt"
    "bufio"
    "os"
    "sort"
    "time"
)

func getNumbers() ([]int, []int) {
    // open file
    file, err := os.Open("../Input.txt")
    if err != nil {
        panic(err)
    }
    defer file.Close()

    // read all lines, save to two arrays, input is 'NUMBER   NUMBER' next line
    scanner := bufio.NewScanner(file)
    numbers1 := make([]int, 0, 2000)
    numbers2 := make([]int, 0, 2000)
    for scanner.Scan() {
        var n1, n2 int
        fmt.Sscanf(scanner.Text(), "%d %d", &n1, &n2)
        numbers1 = append(numbers1, n1)
        numbers2 = append(numbers2, n2)
    }

    // sort the arrays
    sort.Ints(numbers1)
    sort.Ints(numbers2)

    return numbers1, numbers2
}

func abs(x int) int {
    if x < 0 {
        return -x
    }
    return x
}

func calculateTotalDistance(numbers1, numbers2 []int) int {
    // calculate total distance
    totalDistance := 0
    for i := 0; i < max(len(numbers1), len(numbers2)); i++ {
        totalDistance += abs(numbers1[i] - numbers2[i])
    }
    return totalDistance
}

func calculateSimilarity(numbers1, numbers2 []int) int {
    // similarity is sum of elements: numbers1[i] * number of occurences of numbers1[i] in numbers2
    similarity := 0
    occurences := make(map[int]int)

    for i := 0; i < len(numbers2); i++ {
        occurences[numbers2[i]]++
    }

    for i := 0; i < len(numbers1); i++ {
        value, exists := occurences[numbers1[i]]
        if exists {
            similarity += numbers1[i] * value
        }
    }

    return similarity
}

func main() {
    start := time.Now()

    numbers1, numbers2 := getNumbers()
    totalDistance := calculateTotalDistance(numbers1, numbers2)
    similarity := calculateSimilarity(numbers1, numbers2)

    elapsed := time.Since(start)

    fmt.Println("Total distance:", totalDistance)
    fmt.Println("Similarity:", similarity)
    fmt.Println("Execution time:", elapsed)
}
