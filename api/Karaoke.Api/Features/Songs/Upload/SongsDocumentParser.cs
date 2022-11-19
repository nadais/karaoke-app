using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Karaoke.Api.Data;
using OneOf;

namespace Karaoke.Api.Features.Songs.Upload;

public class SongsDocumentParser : ISongsDocumentParser
{
    public OneOf<ICollection<Song>, RequestError> ParseDocumentLines(IFormFile file, string catalogName = "")
    {
        if (!file.FileName.EndsWith("docx"))
        {
            return new RequestError($"Document extension should be docx - received file with name {file.FileName}");
        }

        using var doc = WordprocessingDocument.Open(file.OpenReadStream(), false);
        if (doc.MainDocumentPart?.Document.Body == null)
        {
            return new RequestError("Invalid document");
        }

        var (content, faultyLines) = ParseDocumentTables(doc.MainDocumentPart.Document.Body, catalogName);
        return faultyLines.Any() ?
            new RequestError("The lines in content were faulty", faultyLines) :
            content.Values.ToList();
    }

    private static (Dictionary<string, Song> content, List<string> faultyLines) ParseDocumentTables(OpenXmlElement body, string catalogName)
    {
        var tables = body.Elements<Table>();

        var content = new Dictionary<string, Song>();
        var faultyLines = new List<string>();
        foreach (var table in tables)
        {
            var rows = table.Elements<TableRow>();
            ParseRows(catalogName, rows, content, faultyLines);
        }

        return (content, faultyLines);
    }

    private static void ParseRows(string catalogName, IEnumerable<TableRow> rows,
        IDictionary<string, Song> content, ICollection<string> faultyLines)
    {
        var isFirstRow = true;
        var category = "";
        foreach (var row in rows)
        {
            var currentRow = row.Descendants<TableCell>().Select(cell => cell.InnerText).ToList();
            if (isFirstRow)
            {
                category = currentRow[1];
                isFirstRow = false;
                continue;
            }

            try
            {
                ParseSong(catalogName, content, category, currentRow);
            }
            catch (Exception)
            {
                faultyLines.Add(string.Join(',', currentRow));
            }
        }
    }

    private static void ParseSong(string catalogName, IDictionary<string, Song> content, string category, IReadOnlyList<string> currentRow)
    {
        var categoryName = category is "Title" or "Titre" ? "General" : category;
        var song = new Song
        {
            Artist = currentRow[2].Trim(),
            Name = currentRow[1].Trim(),
            Number = int.Parse(currentRow[0]),
            Categories = new List<string> { categoryName },
            Catalogs = new List<string> { catalogName }
        };
        if (content.ContainsKey(song.Key))
        {
            content[song.Key].Categories.Add(categoryName);
            return;
        }

        content.Add(song.Key, song);
    }
}