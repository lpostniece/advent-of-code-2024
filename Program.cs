using System.Data;
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

/*Console.WriteLine("Advent Day 3");

var pairs = ParseTextForMulDoDont(File.ReadAllText(args[0]));

var multiplied = pairs.Select(pair => pair.Item1 * pair.Item2);

var summed = multiplied.Sum();

Console.WriteLine(summed);*/

Console.WriteLine("Advent Day 4");

var array = ParseXmasGrid(args[0]);

Console.WriteLine(array);

// first dimension moves across
// second dimension moves up

Console.WriteLine(WordOccurences(array, "XMAS"));

int WordOccurences(char[,] array, string word)
{
    int sum = 0;

    for (int i = 0; i < array.GetLength(0); i++)
    {
        for (int j = 0; j < array.GetLength(1); j++)
        {
            int count = CountOfWordStartsAtPosition(array, "XMAS", i,j);
            sum += count;
        }
    }

    return sum;
}

int CountOfWordStartsAtPosition(char[,] array, string word, int x, int y)
{
    var directionList = new List<DirectionEnum>()
    {
        DirectionEnum.Up,
        DirectionEnum.Down,
        DirectionEnum.Right,
        DirectionEnum.Left,
        DirectionEnum.UpRight,
        DirectionEnum.DownRight,
        DirectionEnum.UpLeft,
        DirectionEnum.DownLeft
    };

    int count = 0;

    foreach (var dir in directionList)
    {
        if (DoesWordStartAtPositionWithDirection(array, word, x, y, dir))
        {
            count++; 
        }
    }

    return count;
}

bool DoesWordStartAtPositionWithDirection(char[,] array, string word, int x, int y, DirectionEnum dir)
{
    var letterMatch = array[x, y].Equals(word.First());

    if (letterMatch)
    {
        if (word.Length == 1)
        {
            Console.WriteLine($"DoesWordStartAtPositionWithDirection {word} {x} {y} {dir}");
            return true;
        }
        else
        {
            var (nextX, nextY, isValid) = GetNextPosition(dir, array, x, y);

            if (isValid)
            {
                return DoesWordStartAtPositionWithDirection(array, word.Substring(1), nextX, nextY, dir);
            }
        }
    }

    return false;
}


(int, int, bool) GetNextPosition(DirectionEnum dir, char[,] array, int currentX, int currentY)
{
    int nextX = currentX;
    int nextY = currentY;

    switch(dir)
    {
        case DirectionEnum.Left:
            nextX = nextX - 1;
            break;
        case DirectionEnum.Right:
            nextX = nextX + 1;
            break;
        case DirectionEnum.Up:
            nextY = nextY - 1;
            break;
        case DirectionEnum.Down:
            nextY = nextY + 1;
            break;
        case DirectionEnum.UpRight:
            nextY = nextY - 1;
            nextX = nextX + 1;
            break;
        case DirectionEnum.DownRight:
            nextY = nextY + 1;
            nextX = nextX + 1;
            break;
        case DirectionEnum.DownLeft:
            nextY = nextY + 1;
            nextX = nextX - 1;
            break;
        case DirectionEnum.UpLeft:
            nextY = nextY - 1;
            nextX = nextX - 1;
            break;
    }

    if (nextX >= 0 && nextX < array.GetLength(0) && nextY >= 0 && nextY < array.GetLength(1))
        return (nextX, nextY, true);
    else 
        return (-1, -1, false);
}

static char[,] ParseXmasGrid(string inputFileName)
{
    IEnumerable<char[]> listOfArrays = File.ReadLines(inputFileName).Select(str => str.ToCharArray());
    var lengthOfFirst = listOfArrays.FirstOrDefault().Length;
    if (listOfArrays.Any(arr => arr.Length != lengthOfFirst))
    {
        throw new Exception("Non-rectangular array");
    }

    char[,] array = new char[listOfArrays.Count(), lengthOfFirst];

    for (int down = 0; down < listOfArrays.Count(); down++)
    {
        for (int across = 0; across < lengthOfFirst; across++)
            {
                array[down, across] = listOfArrays.ElementAt(down).ElementAt(across);
            }
    }

    return array;
}

static int ParseMatchGroup(Match match, int index)
{
    string numString = match
                        .Groups[index]
                        .Captures.Aggregate("", 
                                            (accumulator, digit) => accumulator + digit);
    return Int32.Parse(numString);
}

static (int, int) ParsePair(Match match)
{
    if (match.Groups.Count != 3)
    {
        throw new Exception("Expecting 3 groups");
    }
    return (ParseMatchGroup(match, 1), ParseMatchGroup(match, 2));
}

static IEnumerable<(int, int)> ParseTextForMul(string text)
{
    var matches = Regex.Matches(text, @"mul\((\d)+,(\d)+\)");
    return matches.Select(ParsePair);
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
        else if (enabled)
        {
            pairs.Add(ParsePair(match));
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

enum DirectionEnum
{
    Up,
    Down,
    Right,
    Left,
    UpRight,
    DownRight,
    UpLeft,
    DownLeft
}