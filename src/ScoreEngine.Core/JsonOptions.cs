using System.Text.Json;

namespace ScoreEngine.Core;

public static class ScoreEngineJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };
}
