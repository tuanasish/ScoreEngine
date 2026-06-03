namespace ScoreEngine.Core;

public static class WordPropertyResolver
{
    private const string TextRunPrefix = "textRuns.byText['";
    private const string TextRunSuffix = "'].hasDirectFormatting";

    public static object? Resolve(WordDocumentState state, string property)
    {
        return property switch
        {
            "documentProperties.category" => state.DocumentProperties.Category,
            "pageSetup.paperSize" => state.PageSetup.PaperSize,
            "numbering.bullet" => new { state.Numbering.BulletSymbol, state.Numbering.BulletFont },
            "numbering.bullet.symbol" => state.Numbering.BulletSymbol,
            "numbering.bullet.font" => state.Numbering.BulletFont,
            "pictures.artisticEffect" => state.Pictures.ArtisticEffect,
            _ when IsTextRunFormattingProperty(property) => ResolveTextRunFormatting(state, property),
            _ => null
        };
    }

    private static bool IsTextRunFormattingProperty(string property)
    {
        return property.StartsWith(TextRunPrefix, StringComparison.Ordinal)
            && property.EndsWith(TextRunSuffix, StringComparison.Ordinal);
    }

    private static bool? ResolveTextRunFormatting(WordDocumentState state, string property)
    {
        var text = property[TextRunPrefix.Length..^TextRunSuffix.Length];
        return state.TextRuns.FirstOrDefault(run => run.Text == text)?.HasDirectFormatting;
    }
}
