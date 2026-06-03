using System.Text.Json;

namespace ScoreEngine.Core;

public sealed class AnswerKey
{
    public ExamInfo ExamInfo { get; init; } = new();
    public List<Domain> Domains { get; init; } = [];
}

public sealed class ExamInfo
{
    public string Product { get; init; } = "";
    public string Title { get; init; } = "";
    public int TotalPoints { get; init; }
}

public sealed class Domain
{
    public string DomainId { get; init; } = "";
    public string DomainName { get; init; } = "";
    public List<Checkpoint> Checkpoints { get; init; } = [];
}

public sealed class Checkpoint
{
    public string Id { get; init; } = "";
    public string Description { get; init; } = "";
    public int Points { get; init; }
    public string Property { get; init; } = "";
    public JsonElement ExpectedValue { get; init; }
    public string ComparisonType { get; init; } = "";
}
