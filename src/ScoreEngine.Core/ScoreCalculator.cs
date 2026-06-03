namespace ScoreEngine.Core;

public static class ScoreCalculator
{
    public static ScoreReport BuildReport(AnswerKey answerKey, List<DomainScoreReport> domains)
    {
        var totalScore = domains.Sum(domain => domain.Score);
        var maxScore = domains.Sum(domain => domain.MaxScore);

        return new ScoreReport
        {
            Product = answerKey.ExamInfo.Product,
            Title = answerKey.ExamInfo.Title,
            TotalRawScore = totalScore,
            MaxRawScore = maxScore,
            Percentage = GetPercentage(totalScore, maxScore),
            Passed = maxScore > 0 && totalScore == maxScore,
            ExamDate = DateTimeOffset.UtcNow,
            Domains = domains
        };
    }

    public static DomainScoreReport BuildDomainReport(Domain domain, List<CheckpointResult> checkpoints)
    {
        var score = checkpoints.Where(checkpoint => checkpoint.Passed).Sum(checkpoint => checkpoint.Points);
        var maxScore = checkpoints.Sum(checkpoint => checkpoint.Points);

        return new DomainScoreReport
        {
            DomainId = domain.DomainId,
            DomainName = domain.DomainName,
            Score = score,
            MaxScore = maxScore,
            Percentage = GetPercentage(score, maxScore),
            Checkpoints = checkpoints
        };
    }

    private static double GetPercentage(int score, int maxScore)
    {
        return maxScore == 0 ? 0 : Math.Round(score * 100.0 / maxScore, 2);
    }
}
