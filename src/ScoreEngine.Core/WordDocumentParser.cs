using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ScoreEngine.Core;

public sealed class WordDocumentParser
{
    private const long LetterWidth = 12240;
    private const long LetterHeight = 15840;

    public WordDocumentState Parse(string path)
    {
        using var document = WordprocessingDocument.Open(path, false);
        var mainPart = document.MainDocumentPart
            ?? throw new InvalidDataException("Word main document part is missing.");

        var wordDocument = mainPart.Document
            ?? throw new InvalidDataException("Word document root is missing.");

        var body = wordDocument.Body
            ?? throw new InvalidDataException("Word document body is missing.");

        return new WordDocumentState
        {
            DocumentProperties = new DocumentPropertiesState
            {
                Category = document.PackageProperties.Category
            },
            PageSetup = ReadPageSetup(body),
            Numbering = ReadNumbering(mainPart),
            Pictures = ReadPictures(body),
            TextRuns = ReadTextRuns(body)
        };
    }

    private static PageSetupState ReadPageSetup(Body body)
    {
        var pageSize = body.Descendants<SectionProperties>()
            .LastOrDefault()
            ?.GetFirstChild<PageSize>();

        var width = pageSize?.Width?.Value;
        var height = pageSize?.Height?.Value;

        return new PageSetupState
        {
            Width = width,
            Height = height,
            PaperSize = GetPaperSize(width, height)
        };
    }

    private static string? GetPaperSize(long? width, long? height)
    {
        return width == LetterWidth && height == LetterHeight ? "Letter" : null;
    }

    private static NumberingState ReadNumbering(MainDocumentPart? mainPart)
    {
        var levels = mainPart?.NumberingDefinitionsPart?.Numbering
            ?.Descendants<Level>()
            .Where(IsBulletLevel)
            .Select(ReadBulletLevel)
            .Where(level => level.Symbol is not null)
            .ToList() ?? [];

        var selected = levels.FirstOrDefault(level => level.Symbol == "\u2708")
            ?? levels.FirstOrDefault();

        return new NumberingState
        {
            BulletSymbol = selected?.Symbol,
            BulletFont = selected?.Font
        };
    }

    private static bool IsBulletLevel(Level level)
    {
        return level.NumberingFormat?.Val?.Value == NumberFormatValues.Bullet;
    }

    private static BulletLevel ReadBulletLevel(Level level)
    {
        var runFonts = level.GetFirstChild<NumberingSymbolRunProperties>()?.RunFonts;

        return new BulletLevel
        {
            Symbol = level.LevelText?.Val?.Value,
            Font = runFonts?.Ascii?.Value ?? runFonts?.HighAnsi?.Value
        };
    }

    private static PicturesState ReadPictures(Body body)
    {
        var hasGlassEffect = body.Descendants<Drawing>()
            .Any(drawing => drawing.OuterXml.Contains("artisticGlass", StringComparison.OrdinalIgnoreCase));

        return new PicturesState
        {
            ArtisticEffect = hasGlassEffect ? "artisticGlass" : null
        };
    }

    private static List<TextRunState> ReadTextRuns(Body body)
    {
        return body.Descendants<Run>()
            .Select(ReadTextRun)
            .Where(run => run.Text.Length > 0)
            .ToList();
    }

    private static TextRunState ReadTextRun(Run run)
    {
        return new TextRunState
        {
            Text = string.Concat(run.Elements<Text>().Select(text => text.Text)),
            HasDirectFormatting = HasDirectFormatting(run.RunProperties)
        };
    }

    private static bool HasDirectFormatting(RunProperties? runProperties)
    {
        return runProperties?.ChildElements.Any(IsDirectFormattingElement) == true;
    }

    private static bool IsDirectFormattingElement(OpenXmlElement element)
    {
        return element.LocalName is not "noProof" and not "lang";
    }

    private sealed class BulletLevel
    {
        public string? Symbol { get; init; }
        public string? Font { get; init; }
    }
}
