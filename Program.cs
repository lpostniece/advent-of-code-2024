Console.WriteLine("Advent Day 1");

var pair = ParseText(args[0]);
Console.WriteLine(SumOfPairs(pair.Item1, pair.Item2));
Console.WriteLine(SimilarityScore(pair.Item1, pair.Item2));

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

    var sum = 0;

    foreach ((var a, var b) in left.Zip(right))
    {
        sum += (a <= b) ? (b - a) : a - b;
    }

    return sum;
}

static (List<int>, List<int>) ParseText(String fileName)
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
