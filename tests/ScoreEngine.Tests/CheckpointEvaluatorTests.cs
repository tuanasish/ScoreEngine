using System.Text.Json;
using ScoreEngine.Core;

namespace ScoreEngine.Tests;

public sealed class CheckpointEvaluatorTests
{
    [Fact]
    public void Evaluate_passes_exact_string_match()
    {
        var result = CheckpointEvaluator.Evaluate(CreateCheckpoint("\"Memo\"", "exactMatch"), "Memo");

        Assert.True(result.Passed);
    }

    [Fact]
    public void Evaluate_fails_exact_string_mismatch()
    {
        var result = CheckpointEvaluator.Evaluate(CreateCheckpoint("\"Memo\"", "exactMatch"), "Notice");

        Assert.False(result.Passed);
    }

    [Fact]
    public void Evaluate_passes_exists_when_actual_value_is_present()
    {
        var result = CheckpointEvaluator.Evaluate(CreateCheckpoint("null", "exists"), "artisticGlass");

        Assert.True(result.Passed);
    }

    [Fact]
    public void Evaluate_passes_not_exists_when_actual_value_is_missing()
    {
        var result = CheckpointEvaluator.Evaluate(CreateCheckpoint("null", "notExists"), null);

        Assert.True(result.Passed);
    }

    [Fact]
    public void Evaluate_matches_object_values()
    {
        var actual = new { bulletSymbol = "\u2708", bulletFont = "Segoe UI Emoji" };
        var expected = """
            {
              "bulletFont": "Segoe UI Emoji",
              "bulletSymbol": "\u2708"
            }
            """;

        var result = CheckpointEvaluator.Evaluate(CreateCheckpoint(expected, "exactMatch"), actual);

        Assert.True(result.Passed);
    }

    private static Checkpoint CreateCheckpoint(string expectedValueJson, string comparisonType)
    {
        return new Checkpoint
        {
            Id = "test",
            Description = "Test checkpoint",
            Points = 1,
            Property = "test.property",
            ExpectedValue = ReadJsonElement(expectedValueJson),
            ComparisonType = comparisonType
        };
    }

    private static JsonElement ReadJsonElement(string json)
    {
        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }
}
