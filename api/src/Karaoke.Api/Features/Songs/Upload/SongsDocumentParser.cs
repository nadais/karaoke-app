using Karaoke.Api.Data;
using OneOf;

namespace Karaoke.Api.Features.Songs.Upload;

public class SongsDocumentParser : ISongsDocumentParser
{
    private const int NumberIndex = 0;
    private const int NameIndex = 1;
    private const int ArtistIndex = 2;
    public OneOf<ICollection<Song>, RequestError> ParseDocumentLines(List<InternalTable> tables, string catalogName)
    {
        if (string.IsNullOrWhiteSpace(catalogName))
        {
            return new RequestError("Catalog name cannot be empty");
        }
        
        var result = new Dictionary<string, Song>();
        var faultyLines = new List<string>();
        foreach (var table in tables)
        {
            if (table.Headers.Count != 3)
            {
                return new RequestError("Table provided does not have 3 headers", table.Headers);
            }

            var tableResult = ParseContent(table, catalogName, result);
            if (tableResult.IsT1)
            {
                faultyLines.AddRange(tableResult.AsT1);
                continue;
            }

            result = tableResult.AsT0;
        }

        if (faultyLines.Any())
        {
            return new RequestError("The following lines were faulty",
                faultyLines);
        }
        return result.Values;
    }

    private static string GetFaultyLine(IEnumerable<string> row, IReadOnlyList<string> headers)
    {
        return string.Join(";", row.Select((x, i) => $"{headers[i]}:{x}"));
    }

    private static OneOf<Dictionary<string,Song>, List<string>> ParseContent(InternalTable table, string catalogName, Dictionary<string, Song> result)
    {
        var faultyLines = new List<string>();
        var categoryName = table.Headers[NameIndex] is "Title" or "Titre" ? "General" : table.Headers[NameIndex].Trim();
        var categories = new List<string> { categoryName };
        var catalogs = new List<string> { catalogName };

        foreach (var content in table.Content)
        {
            var songResult = GetSong(content, table.Headers);
            if (songResult.IsT1)
            {
                faultyLines.Add(songResult.AsT1);
                continue;
            }
            var song = songResult.AsT0;
            
            if (result.ContainsKey(song.Key))
            {
                result[song.Key].Categories.Add(categoryName);
            }
            else
            {
                song.Categories = categories;
                song.Catalogs = catalogs;
                result.Add(song.Key, song);
            }
        }

        return faultyLines.Count > 0? faultyLines : result;
    }
    
    private static OneOf<Song, string> GetSong(IReadOnlyList<string> content, IReadOnlyList<string> headers)
    {

        var isValid = int.TryParse(content[NumberIndex], out var number);
        if (!isValid)
        {
            return GetFaultyLine(content, headers);
        }
        if (string.IsNullOrWhiteSpace(content[ArtistIndex]))
        {
            return GetFaultyLine(content, headers);
        }
        if (string.IsNullOrWhiteSpace(content[NameIndex]))
        {
            return GetFaultyLine(content, headers);
        }
        return new Song
        {
            Number = number,
            Artist = content[ArtistIndex].Trim(),
            Name = content[NameIndex].Trim()
        };
    }
}