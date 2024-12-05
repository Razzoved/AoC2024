package main

import (
    "fmt"
    "os"
    "time"
    "bufio"
    "strings"
    "strconv"
)

func getReports() [][]int {
    // open file
    file, err := os.Open("../input.txt")
    if err != nil {
        panic(err)
    }
    defer file.Close()

    // read file
    scanner := bufio.NewScanner(file)
    reports := make([][]int, 0, 2000)

    // read each line
    for scanner.Scan() {
        text := scanner.Text()
        if (len(text) == 0 || text[0] == '#') {
            continue
        }
        parts := strings.Fields(scanner.Text())
        report := make([]int, 0, 5)
        for _, part := range parts {
            num, err := strconv.Atoi(part)
            if err != nil {
                panic(err)
            }
            report = append(report, num)
        }
        reports = append(reports, report)
    }

    return reports
}

func isSafe(report []int, direction, lowerBound, upperBound int) int {
    for i := lowerBound; i != upperBound; i += direction {
        a := report[i]
        b := report[i + direction]
        if (a >= b || b - 3 > a) {
            return 0
        }
    }
    return 1
}

func isDamp(report []int, direction, lowerBound, upperBound int) int {
    fmt.Println("Checking", report)
    isDampened := false

    for i := lowerBound; (i > 0 && direction == -1) || (i + 1 < len(report) && direction == 1); i += direction {
        a := report[i]
        b := report[i + direction]

        fmt.Println("Comparing", a, b)
        // ok
        if a < b && a + 3 >= b { continue }

        fmt.Println("Not ok", a, b)
        // nok and already dampened
        if isDampened {
            fmt.Println("Already dampened", a, b)
            return 0
        }

        //fmt.Println("Dampening", a, b)
        isDampened = true
        farIndex := i + direction * 2

        if (farIndex >= 0 && farIndex < len(report) - 1) {
            // far index still in bounds
            if (a < report[farIndex] && a + 3 >= report[farIndex]) {
                // discard b (a -> far holds)
                report[i +  direction] = a
                fmt.Println("Dampening b", a, b, report[farIndex])
            } else if (b < report[farIndex] && b + 3 >= report[farIndex]) {
                // discard a
                fmt.Println("Dampening a", a, b, report[farIndex])
            } else {
                fmt.Println("Dampening failed", a, b, report[farIndex])
                return 0
            }
        }
    }

    return 1
}

func main() {
    start := time.Now()

    reports := getReports()

    reports = [][]int{{27, 29, 32, 33, 36, 37, 40, 37}}

    safeCount := 0
    dampCount := 0

    for _, report := range reports {
        direction := 1
        lowerBound := 0
        upperBound := len(report) - 1
        if (len(report) > 1 && report[0] > report[1]) {
            direction = -1
            lowerBound, upperBound = upperBound, lowerBound
        }
        safeCount += isSafe(report, direction, lowerBound, upperBound)
        dampCount += isDamp(report, direction, lowerBound, upperBound)
    }

    elapsed := time.Since(start)

    fmt.Println("Safe count:", safeCount)
    fmt.Println("Damp count:", dampCount)
    fmt.Println("Execution time:", elapsed)
}
