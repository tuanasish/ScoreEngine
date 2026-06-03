using ScoreEngine.Core;

namespace ScoreEngine.Tests;

public sealed class WordDocumentParserTests
{
    private readonly WordDocumentState completedState = new WordDocumentParser().Parse(TestPaths.CompletedWordSample);

    [Fact]
    public void Parse_reads_document_category()
    {
        Assert.Equal("Memo", completedState.DocumentProperties.Category);
    }

    [Fact]
    public void Parse_detects_letter_paper_size()
    {
        Assert.Equal("Letter", completedState.PageSetup.PaperSize);
        Assert.Equal(12240, completedState.PageSetup.Width);
        Assert.Equal(15840, completedState.PageSetup.Height);
    }

    [Fact]
    public void Parse_detects_plane_bullet_symbol_and_font()
    {
        Assert.Equal("\u2708", completedState.Numbering.BulletSymbol);
        Assert.Equal("Segoe UI Emoji", completedState.Numbering.BulletFont);
    }

    [Fact]
    public void Parse_detects_glass_artistic_effect()
    {
        Assert.Equal("artisticGlass", completedState.Pictures.ArtisticEffect);
    }

    [Fact]
    public void Parse_detects_target_text_without_direct_formatting()
    {
        var targetRun = completedState.TextRuns.Single(run => run.Text == "August Bergqvist, PTA Treasurer");

        Assert.False(targetRun.HasDirectFormatting);
    }
}
