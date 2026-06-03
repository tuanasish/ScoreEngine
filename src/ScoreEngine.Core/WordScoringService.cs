namespace ScoreEngine.Core;

public sealed class WordScoringService
{
    private readonly WordDocumentParser parser;

    public WordScoringService(WordDocumentParser? parser = null)
    {
        this.parser = parser ?? new WordDocumentParser();
    }

    public ScoreReport Score(string studentPath, AnswerKey answerKey)
    {
        var documentState = parser.Parse(studentPath);
        return Score(documentState, answerKey);
    }

    public ScoreReport Score(WordDocumentState documentState, AnswerKey answerKey)
    {
        var domains = answerKey.Domains
            .Select(domain => ScoreDomain(documentState, domain))
            .ToList();

        return ScoreCalculator.BuildReport(answerKey, domains);
    }

    private static DomainScoreReport ScoreDomain(WordDocumentState documentState, Domain domain)
    {
        var checkpoints = domain.Checkpoints
            .Select(checkpoint => EvaluateCheckpoint(documentState, checkpoint))
            .ToList();

        return ScoreCalculator.BuildDomainReport(domain, checkpoints);
    }

    private static CheckpointResult EvaluateCheckpoint(WordDocumentState documentState, Checkpoint checkpoint)
    {
        var actualValue = WordPropertyResolver.Resolve(documentState, checkpoint.Property);
        return CheckpointEvaluator.Evaluate(checkpoint, actualValue);
    }
}
