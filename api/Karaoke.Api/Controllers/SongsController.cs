using System.Text.Json;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using OneOf;

namespace Karaoke_catalog_server.Controllers;

[ApiController]
[Route("[controller]")]
public class SongsController : ControllerBase
{
    private readonly IDistributedCache _redisCache;
    private static Dictionary<string, string> _localCache = new();
    private const string CacheEntryName = "catalog";

    public SongsController(
        IDistributedCache redisCache
        )
    {
        _redisCache = redisCache;
    }
    public record Song(string Artist, string Name, int Number, string? Category);

    public record RequestError(string? Message = null, object? Content = null);

    public record Catalog(Dictionary<string, ICollection<Song>> SongGroups);

    private async Task<Catalog> GetSongsFromCache()
    {
        // Line here for debugging purposes locally
        // var content = _localCache.ContainsKey(CacheEntryName) ? _localCache[CacheEntryName] : null;
        var content = await _redisCache.GetStringAsync(CacheEntryName);
        if (content == null)
        {
            return new Catalog(new Dictionary<string, ICollection<Song>>());
        }
        var songs = JsonSerializer.Deserialize<Catalog>(content);
        return songs ?? new Catalog(new Dictionary<string, ICollection<Song>>());
    }
    [HttpGet(Name = "GetSongs")]
    public async Task<Catalog> GetSongs()
    {
        return await GetSongsFromCache();
    }

    [HttpPost("clear")]
    public async Task ClearCache()
    {
        await _redisCache.RemoveAsync(CacheEntryName);
    }

    [HttpPost("validate")]
    public IActionResult Validate([FromForm] IFormFile file)
    {
        var result = ParseDocumentLines(file);
        return result.Match<IActionResult>(Ok, UnprocessableEntity );
    }

    [HttpPost("{tagName}", Name = "UploadList")]
    public async Task<IActionResult> Post(string tagName, [FromForm] IFormFile file)
    {
        tagName = Uri.UnescapeDataString(tagName);
        var result = ParseDocumentLines(file);

        return await result.Match<Task<IActionResult>>(async content =>
        {
            var storedCatalog = await GetSongsFromCache();
            if (storedCatalog.SongGroups.ContainsKey(tagName))
            {
                storedCatalog.SongGroups.Remove(tagName);
            }
            storedCatalog.SongGroups.Add(tagName, content);

            var cacheContent = JsonSerializer.Serialize(storedCatalog);

            // Line here for debugging purposes locally
            _localCache[CacheEntryName] = cacheContent;
            // await _redisCache.SetStringAsync(CacheEntryName, cacheContent);
            return Ok(cacheContent);
        },async request => await Task.FromResult(BadRequest(request)));
    }

    private static OneOf<List<Song>, RequestError> ParseDocumentLines(IFormFile file)
    {
        if (!file.FileName.EndsWith("docx"))
        {
            return new RequestError($"Document extension should be docx - received file with name {file.FileName}");
        }

        using WordprocessingDocument doc = WordprocessingDocument.Open(file.OpenReadStream(), false);
        // Find the first table in the document.   
        var tables = doc.MainDocumentPart.Document.Body.Elements<Table>();

        // To get all rows from table
        var content = new List<Song>();
        var faultyLines = new List<string>();
        foreach (var table in tables)
        {
            var rows = table.Elements<TableRow>();
            // To read data from rows and to add records to the temporary table
            var isFirstRow = true;
            var category = "";
            foreach (var row in rows)
            {
                var currentRow = new List<string>();
                currentRow.AddRange(row.Descendants<TableCell>().Select(cell => cell.InnerText));
                if (isFirstRow)
                {
                    category = currentRow[1];
                    isFirstRow = false;
                    continue;
                }

                try
                {
                    content.Add(new Song(currentRow[2].Trim(), currentRow[1].Trim(), int.Parse(currentRow[0]),
                        category is "Title" or "Titre" ? "General" : category));
                }
                catch (Exception)
                {
                    faultyLines.Add(string.Join(',', currentRow));
                }
            }
        }
        return faultyLines.Any() ? new RequestError("The lines in content were faulty", faultyLines) : content;
    }
}