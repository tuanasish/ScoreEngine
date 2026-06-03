namespace ScoreEngine.Tests;

internal static class TestPaths
{
    public static string ScoreEngineRoot => FindScoreEngineRoot();

    public static string CompletedWordSample =>
        Path.Combine(ScoreEngineRoot, "sample-data", "word", "project1_completed.docx");

    public static string InitialWordSample =>
        Path.Combine(ScoreEngineRoot, "sample-data", "word", "project1_initial.docx");

    public static string Project1AnswerKey =>
        Path.Combine(ScoreEngineRoot, "sample-data", "word", "project1_answer_key.json");

    private static string FindScoreEngineRoot()
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "ScoreEngine.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate ScoreEngine root.");
    }
}
