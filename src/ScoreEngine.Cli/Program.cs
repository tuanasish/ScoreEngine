using System.Text.Json;
using ScoreEngine.Core;

return Run(args);

static int Run(string[] args)
{
    try
    {
        var command = CliCommand.Parse(args);
        var report = Score(command);
        var json = JsonSerializer.Serialize(report, ScoreEngineJson.Options);

        Console.WriteLine(json);
        WriteOutput(command.OutputPath, json);
        return 0;
    }
    catch (Exception exception) when (exception is not JsonException)
    {
        Console.Error.WriteLine(exception.Message);
        return 1;
    }
    catch (JsonException exception)
    {
        Console.Error.WriteLine($"Invalid answer key JSON: {exception.Message}");
        return 1;
    }
}

static ScoreReport Score(CliCommand command)
{
    if (!File.Exists(command.StudentPath))
    {
        throw new FileNotFoundException($"Student file not found: {command.StudentPath}");
    }

    if (!File.Exists(command.AnswerPath))
    {
        throw new FileNotFoundException($"Answer key not found: {command.AnswerPath}");
    }

    var answerKey = AnswerKeyLoader.Load(command.AnswerPath);
    return new WordScoringService().Score(command.StudentPath, answerKey);
}

static void WriteOutput(string? outputPath, string json)
{
    if (string.IsNullOrWhiteSpace(outputPath))
    {
        return;
    }

    var directory = Path.GetDirectoryName(outputPath);

    if (!string.IsNullOrWhiteSpace(directory))
    {
        Directory.CreateDirectory(directory);
    }

    File.WriteAllText(outputPath, json);
}

internal sealed class CliCommand
{
    public string StudentPath { get; private init; } = "";
    public string AnswerPath { get; private init; } = "";
    public string? OutputPath { get; private init; }

    public static CliCommand Parse(string[] args)
    {
        if (args.Length < 2 || args[0] != "word" || args[1] != "score")
        {
            throw new InvalidOperationException("Usage: word score --student <path> --answer <path> [--output <path>]");
        }

        var options = ParseOptions(args.Skip(2).ToArray());

        if (!options.TryGetValue("--student", out var studentPath))
        {
            throw new InvalidOperationException("Missing required option: --student");
        }

        if (!options.TryGetValue("--answer", out var answerPath))
        {
            throw new InvalidOperationException("Missing required option: --answer");
        }

        return new CliCommand
        {
            StudentPath = studentPath,
            AnswerPath = answerPath,
            OutputPath = options.GetValueOrDefault("--output")
        };
    }

    private static Dictionary<string, string> ParseOptions(string[] args)
    {
        var options = new Dictionary<string, string>(StringComparer.Ordinal);

        for (var index = 0; index < args.Length; index += 2)
        {
            if (index + 1 >= args.Length)
            {
                throw new InvalidOperationException($"Missing value for option: {args[index]}");
            }

            options[args[index]] = args[index + 1];
        }

        return options;
    }
}
