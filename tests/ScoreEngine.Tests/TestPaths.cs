namespace ScoreEngine.Tests;

internal static class TestPaths
{
    public static string ScoreEngineRoot => FindScoreEngineRoot();
    public static string DocumentsRoot => FindDocumentsRoot();

    public static string CompletedWordSample =>
        Path.Combine(DocumentsRoot, "inputs", "Project 1 - Sau khi làm.docx");

    public static string InitialWordSample =>
        Path.Combine(DocumentsRoot, "inputs", "Project 1 - Trước khi làm.docx");

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

    private static string FindDocumentsRoot()
    {
        var parent = Directory.GetParent(ScoreEngineRoot)?.FullName
            ?? throw new DirectoryNotFoundException("Could not locate Exam Platform root.");

        var documentsRoot = Path.Combine(parent, "Exam-Platform-Documents");

        if (!Directory.Exists(Path.Combine(documentsRoot, "inputs")))
        {
            throw new DirectoryNotFoundException($"Could not locate sample inputs at {documentsRoot}.");
        }

        return documentsRoot;
    }
}
