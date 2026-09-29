namespace SrpLab;

public sealed class GradeBook
{
    private readonly Dictionary<string, List<decimal>> _scores =
        new(StringComparer.OrdinalIgnoreCase);

    public void Record(string studentId, decimal score)
    {
        if (score is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(score));

        if (!_scores.TryGetValue(studentId, out var list))
        {
            list = new List<decimal>();
            _scores[studentId] = list;
        }

        list.Add(score);
    }

    public IReadOnlyList<decimal> GetScores(string studentId)
    {
        if (!_scores.TryGetValue(studentId, out var list))
            return Array.Empty<decimal>();

        return list;
    }

    public IEnumerable<string> StudentIds => _scores.Keys;
}

public sealed class GradingCalculator
{
    public decimal Average(IReadOnlyList<decimal> scores)
    {
        if (scores.Count == 0)
            return 0m;

        return Math.Round(scores.Average(), 2);
    }

    public string Letter(IReadOnlyList<decimal> scores)
    {
        var average = Average(scores);

        if (average >= 90)
            return "A";

        if (average >= 80)
            return "B";

        if (average >= 70)
            return "C";

        if (average >= 60)
            return "D";

        return "F";
    }
}

public sealed class HonorRollEligibility
{
    private readonly GradingCalculator _gradingCalculator = new();

    public bool Meets(IReadOnlyList<decimal> scores)
    {
        var average = _gradingCalculator.Average(scores);
        var letter = _gradingCalculator.Letter(scores);

        return average >= 85 && letter is "A" or "B";
    }
}

public sealed class TranscriptGenerator
{
    private readonly GradingCalculator _gradingCalculator = new();
    private readonly HonorRollEligibility _honorRollEligibility = new();

    public string Generate(
        string studentId,
        string fullName,
        IReadOnlyList<decimal> scores)
    {
        var average = _gradingCalculator.Average(scores);
        var letter = _gradingCalculator.Letter(scores);
        var honor = _honorRollEligibility.Meets(scores);

        return $"TRANSCRIPT\n" +
               $"Student: {fullName} ({studentId})\n" +
               $"Average: {average}\n" +
               $"Letter: {letter}\n" +
               $"Honor: {honor}\n";
    }
}

public sealed class CsvExporter
{
    private readonly GradingCalculator _gradingCalculator = new();
    private readonly HonorRollEligibility _honorRollEligibility = new();

    public string Export(GradeBook gradeBook)
    {
        var rows = new List<string>
        {
            "studentId,average,letter,honor"
        };

        foreach (var id in gradeBook.StudentIds.OrderBy(x => x))
        {
            var scores = gradeBook.GetScores(id);
            var average = _gradingCalculator.Average(scores);
            var letter = _gradingCalculator.Letter(scores);
            var honor = _honorRollEligibility.Meets(scores);

            rows.Add($"{id},{average},{letter},{(honor ? 1 : 0)}");
        }

        return string.Join('\n', rows);
    }
}