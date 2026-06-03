using System.Text.Json;

namespace ScoreEngine.Core;

public static class AnswerKeyLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public static AnswerKey Load(string path)
    {
        using var stream = File.OpenRead(path);
        var answerKey = JsonSerializer.Deserialize<AnswerKey>(stream, JsonOptions);

        if (answerKey is null)
        {
            throw new InvalidDataException("Answer key JSON is empty or invalid.");
        }

        Validate(answerKey);
        return answerKey;
    }

    private static void Validate(AnswerKey answerKey)
    {
        if (string.IsNullOrWhiteSpace(answerKey.ExamInfo.Product))
        {
            throw new InvalidDataException("Answer key is missing examInfo.product.");
        }

        if (answerKey.Domains.Count == 0)
        {
            throw new InvalidDataException("Answer key must contain at least one domain.");
        }

        foreach (var checkpoint in answerKey.Domains.SelectMany(domain => domain.Checkpoints))
        {
            if (string.IsNullOrWhiteSpace(checkpoint.Id))
            {
                throw new InvalidDataException("Every checkpoint must have an id.");
            }

            if (string.IsNullOrWhiteSpace(checkpoint.Property))
            {
                throw new InvalidDataException($"Checkpoint {checkpoint.Id} is missing property.");
            }

            if (string.IsNullOrWhiteSpace(checkpoint.ComparisonType))
            {
                throw new InvalidDataException($"Checkpoint {checkpoint.Id} is missing comparisonType.");
            }
        }
    }
}
