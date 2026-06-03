namespace ScoreEngine.Core;

public sealed class ScoreReport
{
    public string Product { get; init; } = "";
    public string Title { get; init; } = "";
    public int TotalRawScore { get; init; }
    public int MaxRawScore { get; init; }
    public double Percentage { get; init; }
    public bool Passed { get; init; }
    public DateTimeOffset ExamDate { get; init; }
    public List<DomainScoreReport> Domains { get; init; } = [];
}

public sealed class DomainScoreReport
{
    public string DomainId { get; init; } = "";
    public string DomainName { get; init; } = "";
    public int Score { get; init; }
    public int MaxScore { get; init; }
    public double Percentage { get; init; }
    public List<CheckpointResult> Checkpoints { get; init; } = [];
}

public sealed class CheckpointResult
{
    public string Id { get; init; } = "";
    public string Description { get; init; } = "";
    public int Points { get; init; }
    public bool Passed { get; init; }
    public object? ActualValue { get; init; }
    public object? ExpectedValue { get; init; }
    public string ComparisonType { get; init; } = "";
}
