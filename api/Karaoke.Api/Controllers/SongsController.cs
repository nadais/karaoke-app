using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Karaoke_catalog_server.Controllers;

[ApiController]
[Route("[controller]")]
public class SongsController : ControllerBase
{
    private readonly IMemoryCache _cache;
    private const string CacheEntryName = "songs";

    public SongsController(IMemoryCache memoryCache)
    {
        _cache = memoryCache;
    }
    public record Song(string Artist, string Name);
    
    [HttpGet(Name = "GetSongs")]
    public async Task<IEnumerable<Song>> GetSongs()
    {
        if (!_cache.TryGetValue(CacheEntryName, out var content) || content == null)
        {
            return new List<Song>();
        }

        var songs = JsonSerializer.Deserialize<List<Song>>((string) content);
        return songs;
    }    
    [HttpPost(Name = "UploadList")]
    public async Task<string> Post([FromForm] IFormFile file)
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

        var filteredContent = content.Where(x => int.TryParse(x[0], out _))
            .Select(x => new Song(x[2], x[1])).ToList();
        var cacheContent = JsonSerializer.Serialize(filteredContent);
        _cache.Set(CacheEntryName, cacheContent);
        return cacheContent;
    }
}