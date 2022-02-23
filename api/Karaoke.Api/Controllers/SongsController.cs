using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Karaoke_catalog_server.Controllers;

[ApiController]
[Route("[controller]")]
public class SongsController : ControllerBase
{
    private readonly IDistributedCache _redisCache;
    private static Dictionary<string, string> _localCache = new();
    private const string CacheEntryName = "catalog";

    public SongsController(IDistributedCache redisCache)
    {
        _redisCache = redisCache;
    }
    public record Song(string Artist, string Name, int Number);

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

    [HttpPost("{tagName}", Name = "UploadList")]
    public async Task<string> Post(string tagName, [FromForm] IFormFile file)
    {
        if (!file.FileName.EndsWith("docx"))
        {
            return string.Empty;
        }
        
        using WordprocessingDocument doc = WordprocessingDocument.Open(file.OpenReadStream(), false);
        // Find the first table in the document.   
        var tables = doc.MainDocumentPart.Document.Body.Elements<Table>();  
  
        // To get all rows from table
        var content = new List<List<string>>();
        foreach (var table in tables)
        {
            IEnumerable<TableRow> rows = table.Elements<TableRow>();  
            // To read data from rows and to add records to the temporary table  
            foreach (TableRow row in rows)
            {
                var currentRow = new List<string>();
                currentRow.AddRange(row.Descendants<TableCell>().Select(cell => cell.InnerText));
                content.Add(currentRow);
            }
        }

        var storedCatalog = await GetSongsFromCache();
        if (storedCatalog.SongGroups.ContainsKey(tagName))
        {
            storedCatalog.SongGroups.Remove(tagName);
        }
        var filteredContent = content.Where(x => int.TryParse(x[0], out _))
            .Select(x => new Song(x[2].Trim(), x[1].Trim(), int.Parse(x[0]))).ToList();
        storedCatalog.SongGroups.Add(tagName, filteredContent);

        var cacheContent = JsonSerializer.Serialize(storedCatalog);

        // Line here for debugging purposes locally
        // _localCache[CacheEntryName] = cacheContent;
        await _redisCache.SetStringAsync(CacheEntryName, cacheContent);
        return cacheContent;
    }
}