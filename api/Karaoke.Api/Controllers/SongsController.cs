using System.Text.Json;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Karaoke_catalog_server.Controllers;

[ApiController]
[Route("[controller]")]
public class SongsController : ControllerBase
{
    private readonly IDistributedCache _redisCache;
    private const string CacheEntryName = "songs";

    public SongsController(IDistributedCache redisCache)
    {
        _redisCache = redisCache;
    }
    public record Song(string Artist, string Name, int Number);
    
    [HttpGet(Name = "GetSongs")]
    public async Task<IEnumerable<Song>> GetSongs()
    {
        try
        {
            var content = await _redisCache.GetStringAsync(CacheEntryName);
            var songs = JsonSerializer.Deserialize<List<Song>>(content);
            return songs;
        }
        catch (Exception)
        {
            return new List<Song>();
        }
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
            .Select(x => new Song(x[2].Trim(), x[1].Trim(), int.Parse(x[0]))).ToList();
        var cacheContent = JsonSerializer.Serialize(filteredContent);
        await _redisCache.SetStringAsync(CacheEntryName, cacheContent);
        return cacheContent;
    }
}