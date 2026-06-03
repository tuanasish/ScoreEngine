namespace ScoreEngine.Core;

public sealed class WordDocumentState
{
    public DocumentPropertiesState DocumentProperties { get; init; } = new();
    public PageSetupState PageSetup { get; init; } = new();
    public NumberingState Numbering { get; init; } = new();
    public PicturesState Pictures { get; init; } = new();
    public List<TextRunState> TextRuns { get; init; } = [];
}

public sealed class DocumentPropertiesState
{
    public string? Category { get; init; }
}

public sealed class PageSetupState
{
    public string? PaperSize { get; init; }
    public long? Width { get; init; }
    public long? Height { get; init; }
}

public sealed class NumberingState
{
    public string? BulletSymbol { get; init; }
    public string? BulletFont { get; init; }
}

public sealed class PicturesState
{
    public string? ArtisticEffect { get; init; }
}

public sealed class TextRunState
{
    public string Text { get; init; } = "";
    public bool HasDirectFormatting { get; init; }
}
