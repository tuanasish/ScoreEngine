using System.Text.Json;

namespace ScoreEngine.Core;

public static class CheckpointEvaluator
{
    public static CheckpointResult Evaluate(Checkpoint checkpoint, object? actualValue)
    {
        var passed = checkpoint.ComparisonType switch
        {
            "exactMatch" => Matches(actualValue, checkpoint.ExpectedValue),
            "exists" => actualValue is not null,
            "notExists" => actualValue is null,
            _ => throw new NotSupportedException($"Unsupported comparisonType: {checkpoint.ComparisonType}")
        };

        return new CheckpointResult
        {
            Id = checkpoint.Id,
            Description = checkpoint.Description,
            Points = checkpoint.Points,
            Passed = passed,
            ActualValue = actualValue,
            ExpectedValue = ToReportValue(checkpoint.ExpectedValue),
            ComparisonType = checkpoint.ComparisonType
        };
    }

    private static bool Matches(object? actualValue, JsonElement expectedValue)
    {
        return expectedValue.ValueKind switch
        {
            JsonValueKind.String => string.Equals(actualValue?.ToString(), expectedValue.GetString(), StringComparison.Ordinal),
            JsonValueKind.True => actualValue is true,
            JsonValueKind.False => actualValue is false,
            JsonValueKind.Null => actualValue is null,
            JsonValueKind.Number => MatchesNumber(actualValue, expectedValue),
            _ => MatchesJson(actualValue, expectedValue)
        };
    }

    private static bool MatchesNumber(object? actualValue, JsonElement expectedValue)
    {
        if (actualValue is null || !expectedValue.TryGetDecimal(out var expectedNumber))
        {
            return false;
        }

        return Convert.ToDecimal(actualValue) == expectedNumber;
    }

    private static bool MatchesJson(object? actualValue, JsonElement expectedValue)
    {
        if (actualValue is null)
        {
            return expectedValue.ValueKind == JsonValueKind.Null;
        }

        var actualJson = JsonSerializer.SerializeToElement(actualValue, ScoreEngineJson.Options);
        return JsonElementEquals(actualJson, expectedValue);
    }

    private static bool JsonElementEquals(JsonElement left, JsonElement right)
    {
        if (left.ValueKind != right.ValueKind)
        {
            return false;
        }

        return left.ValueKind switch
        {
            JsonValueKind.Object => ObjectEquals(left, right),
            JsonValueKind.Array => ArrayEquals(left, right),
            JsonValueKind.String => left.GetString() == right.GetString(),
            JsonValueKind.Number => left.GetDecimal() == right.GetDecimal(),
            JsonValueKind.True or JsonValueKind.False => left.GetBoolean() == right.GetBoolean(),
            JsonValueKind.Null => true,
            _ => left.GetRawText() == right.GetRawText()
        };
    }

    private static bool ObjectEquals(JsonElement left, JsonElement right)
    {
        var leftProperties = left.EnumerateObject().ToDictionary(property => property.Name, property => property.Value);
        var rightProperties = right.EnumerateObject().ToDictionary(property => property.Name, property => property.Value);

        if (leftProperties.Count != rightProperties.Count)
        {
            return false;
        }

        return leftProperties.All(property =>
            rightProperties.TryGetValue(property.Key, out var rightValue)
            && JsonElementEquals(property.Value, rightValue));
    }

    private static bool ArrayEquals(JsonElement left, JsonElement right)
    {
        var leftItems = left.EnumerateArray().ToArray();
        var rightItems = right.EnumerateArray().ToArray();

        if (leftItems.Length != rightItems.Length)
        {
            return false;
        }

        return leftItems.Zip(rightItems).All(pair => JsonElementEquals(pair.First, pair.Second));
    }

    private static object? ToReportValue(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            JsonValueKind.Number when value.TryGetInt32(out var intValue) => intValue,
            JsonValueKind.Number when value.TryGetDouble(out var doubleValue) => doubleValue,
            _ => JsonSerializer.Deserialize<object>(value.GetRawText(), ScoreEngineJson.Options)
        };
    }
}
