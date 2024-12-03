﻿using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

/*Console.WriteLine("Advent Day 1");

var pair = ParseDay1Text(args[0]);
Console.WriteLine(SumOfPairs(pair.Item1, pair.Item2));
Console.WriteLine(SimilarityScore(pair.Item1, pair.Item2));*/

/*
Console.WriteLine("Advent Day 2");
var levels = ParseDay2Text(args[0]);
Console.WriteLine("Normal safety: " + levels.Where(IsLevelSafe).Count());
Console.WriteLine("Safety with dampening: " + levels.Where(IsLevelSafeWithDampening).Count());*/

Console.WriteLine("Advent Day 3");

var pairs = ParseTextForMulDoDont(File.ReadAllText(args[0]));

var multiplied = pairs.Select(pair => pair.Item1 * pair.Item2);

var summed = multiplied.Sum();

Console.WriteLine(summed);

static IEnumerable<(int, int)> ParseTextForMul(string text)
{
    var matches = Regex.Matches(text, @"mul\((\d)+,(\d)+\)");

    var pairs = new List<(int, int)>();

    foreach (Match match in matches)
    {
        //Console.WriteLine(match.Value);
        if (match.Groups.Count != 3)
        {
            throw new Exception("Expecting 3 groups");
        }
        var firstGroup = match.Groups[1];
        string firstNum = "";
        foreach (var digit in firstGroup.Captures)
        {
            firstNum += digit;
        }
        //Console.WriteLine(Int32.Parse(firstNum));

        var secondGroup = match.Groups[2];
        string secondNum = "";
        foreach (var digit in secondGroup.Captures)
        {
            secondNum += digit;
        }
        //Console.WriteLine(Int32.Parse(secondNum));

        pairs.Add((Int32.Parse(firstNum), Int32.Parse(secondNum)));

        //Console.WriteLine(secondGroup.Captures.ToString());
    }

    return pairs;
}

static IEnumerable<(int, int)> ParseTextForMulDoDont(string text)
{
    var matches = Regex.Matches(text, @"do\(\)|mul\((\d)+,(\d)+\)|don't\(\)");

    var pairs = new List<(int, int)>();

    bool enabled = true;

    foreach (Match match in matches)
    {
        if (match.Value == "do()")
        {
            enabled = true;
        }
        else if (match.Value == "don't()")
        {
            enabled = false;
        }
        else
        {
            if (enabled)
            {
                if (match.Groups.Count != 3)
                {
                    throw new Exception("Expecting 3 groups");
                }
                var firstGroup = match.Groups[1];
                string firstNum = "";
                foreach (var digit in firstGroup.Captures)
                {
                    firstNum += digit;
                }

                var secondGroup = match.Groups[2];
                string secondNum = "";
                foreach (var digit in secondGroup.Captures)
                {
                    secondNum += digit;
                }

                pairs.Add((Int32.Parse(firstNum), Int32.Parse(secondNum)));
            }
        }
    }

    return pairs;
}



static bool IsGapSafe(int a, int b)
{
    var diff = Math.Abs(a - b);
    return diff >= 1 && diff <= 3;
}

static bool IsLevelSafeWithDampening(IEnumerable<int> level)
{
    for (var excludeIndex = 0; excludeIndex < level.Count(); excludeIndex++)
    {
        IEnumerable<int> levelWithDampening = level.Take(excludeIndex).Concat(level.TakeLast(level.Count() - excludeIndex - 1));
        if (IsLevelSafe(levelWithDampening))
        {
            return true;
        }
    }

    return false;
}

static bool IsLevelSafe(IEnumerable<int> level)
{
    // special case - 0 or 1 always safe
    if (level.Count() <= 1)
    {
        return true;
    }

    // general case, first gap sets the pattern for ascending or descending

    bool ascending = level.ElementAt(0) < level.ElementAt(1);

    bool allSafe = true;
    for (int i = 0; i < level.Count() - 1; i++)
    {
        allSafe &= IsGapSafe(level.ElementAt(i), level.ElementAt(i+1)) 
                && (ascending ? 
                              (level.ElementAt(i) < level.ElementAt(i+1)) 
                              : level.ElementAt(i) > level.ElementAt(i + 1));
    }

    return allSafe;
}

static IEnumerable<int> LineToArray(String line)
{
    string[] arr = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    return arr.Select(Int32.Parse);
}

static IEnumerable<IEnumerable <int>> ParseDay2Text(String fileName)
{
    return File.ReadLines(fileName).Select(LineToArray);
}

static int SimilarityScore(List<int> left, List<int> right)
{
    var sum = 0;
    foreach (var a in left)
    {
        var numRightOccurences = right.Where(b => b == a).Count();
        sum += a * numRightOccurences;
    }

    return sum;

}

static int SumOfPairs(List<int> left, List<int> right)
{
    left.Sort();
    right.Sort();

    return left.Zip(right).Aggregate(0,
                                    (s, pair) =>
                                        (pair.Item1 <= pair.Item2)
                                        ? s + (pair.Item2 - pair.Item1)
                                        : s + (pair.Item1 - pair.Item2));
}

static (List<int>, List<int>) ParseDay1Text(String fileName)
{
    var left = new List<int>();
    var right = new List<int>();

    var listOfPairs = File.ReadLines(fileName).Select(LineToPair);

    foreach (var (a, b) in listOfPairs)
    {
        left.Add(a);
        right.Add(b);
    } 

    return (left, right);
}

static (int, int) LineToPair(String line)
{
    string[] pair = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    return (Int32.Parse(pair[0].Trim()), Int32.Parse(pair[1].Trim()));
}
