using ScoreEngine.Core;

namespace ScoreEngine.Tests;

public sealed class WordScoringServiceTests
{
    [Fact]
    public void Score_returns_full_score_for_completed_sample()
    {
        var answerKey = AnswerKeyLoader.Load(TestPaths.Project1AnswerKey);
        var report = new WordScoringService().Score(TestPaths.CompletedWordSample, answerKey);

        Assert.Equal(5, report.TotalRawScore);
        Assert.Equal(5, report.MaxRawScore);
        Assert.Equal(100, report.Percentage);
        Assert.True(report.Passed);
        Assert.Equal(5, report.Domains.SelectMany(domain => domain.Checkpoints).Count());
        Assert.All(report.Domains.SelectMany(domain => domain.Checkpoints), checkpoint => Assert.True(checkpoint.Passed));
    }

    [Fact]
    public void Score_fails_sample_changes_for_initial_file()
    {
        var answerKey = AnswerKeyLoader.Load(TestPaths.Project1AnswerKey);
        var report = new WordScoringService().Score(TestPaths.InitialWordSample, answerKey);

        Assert.True(report.TotalRawScore < report.MaxRawScore);
        Assert.False(report.Passed);
    }
}
