using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OneOf;

namespace Karaoke.Api.Features.Songs.Upload;

public class OpenXmlWrapper : IOpenXmlWrapper
{
    
    public OneOf<List<InternalTable>, RequestError> ParseLines(IFormFile file)
    {
        if (!file.FileName.EndsWith("docx"))
        {
            return new RequestError($"Document extension should be docx - received file with name {file.FileName}");
        }

        var readStream = file.OpenReadStream();
        if (readStream.Length == 0)
        {
            return new RequestError("Stream cannot be empty");
        }

        try
        {
            using var doc = WordprocessingDocument.Open(readStream, false);
            if (doc.MainDocumentPart?.Document.Body == null)
            {
                return new RequestError("Invalid document");
            }
            var tables = ParseAllTables(doc.MainDocumentPart.Document.Body);
            return tables;
        }
        catch (Exception)
        {
            return new RequestError("Invalid document");
        }
    }

    private static List<InternalTable> ParseAllTables(OpenXmlElement body)
    {
        var tables = body.Elements<Table>();

        return tables.Select(ParseTable).ToList();
    }

    private static InternalTable ParseTable(OpenXmlElement table)
    {
        var rows = table.Elements<TableRow>();
        var isFirstRow = true;
        var headers = new List<string>();
        var rowsList = rows.ToList();
        var content = new List<List<string>>(rowsList.Count);
        foreach (var row in rowsList)
        {
            var currentRow = row.Descendants<TableCell>().Select(cell => cell.InnerText).ToList();
            if (isFirstRow)
            {
                headers = currentRow;
                isFirstRow = false;
                continue;
            }
            content.Add(currentRow);
        }

        return new InternalTable(headers, content);
    }
}