using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Karaoke.Api.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OneOf;

namespace Karaoke.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SongsController : ControllerBase
{
    private readonly DeezerClient _deezerClient;
    private readonly SongsCollectionSettings _settings;
    private readonly IMongoCollection<Song> _songsCollection;
    public SongsController(
        MongoDbService mongoDbService,
        DeezerClient deezerClient,
        IOptions<SongsCollectionSettings> settings)
    {
        _deezerClient = deezerClient;
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
        _songsCollection = mongoDbService.GetSongsCollection();
    }

    public record RequestError(string? Message = null, object? Content = null);

    public record Catalog(ICollection<Song> SongGroups);

    [HttpGet(Name = "GetSongs")]
    public async Task<Catalog> GetSongs()
    {
        var songs = await  _songsCollection.Find(_ => true).ToListAsync();
        return new Catalog(songs);
    }

    public record StatusResponse(List<string> Duplicates, List<string> AvailableNumbers);

    [HttpGet("status",Name = "FindDuplicates")]
    public async Task<StatusResponse> GetDuplicates()
    {
        var result = _songsCollection.Aggregate()
            .Group(
                x => x.Number,
                x =>
                    new
                    {
                        x.Key,
                        Entries = x.Count()
                    })
            .ToList()
            .Where(x => x.Entries > 1)
            .Select(x => x.Key)
            .ToList();
        var songs = (await _songsCollection.FindAsync(x => result.Contains(x.Number)))
            .ToList()
            .OrderBy(x => x.Number)
            .Select(x => $"{x.Number}_{x.Artist}_{x.Name}")
            .ToList();
        var songNumbers = await _songsCollection.Find(_ => true)
            .Project(x => x.Number)
            .ToListAsync();
        songNumbers = songNumbers.OrderBy(x => x).ToList();
        var startNumber = 1;
        var missingNumbers = new List<string>();
        foreach (var songNumber in songNumbers)
        {
            if (startNumber == songNumber)
            {
                startNumber++;
                continue;
            }

            if (startNumber >= songNumber)
            {
                continue;
            }

            missingNumbers.Add(startNumber == songNumber - 1 ?
                $"{startNumber}" :
                $"{startNumber}-{songNumber - 1}");
            startNumber = songNumber + 1;
        }
        missingNumbers.Add($"{startNumber}+");
        return new StatusResponse(songs, missingNumbers);
    }
    
    [HttpPost("merge")]
    public async Task Merge()
    {
        await MergeSongs();
    }

    [HttpPost("clear")]
    public async Task ClearCache()
    {
        await _songsCollection.Database.DropCollectionAsync(_settings.SongsCollectionName);
        await _songsCollection.Database.CreateCollectionAsync(_settings.SongsCollectionName);
    }

    [HttpPost("{tagName}", Name = "UploadList")]
    public async Task<IActionResult> Post(string tagName, [FromForm] IFormFile file)
    {
        tagName = Uri.UnescapeDataString(tagName);
        var result = ParseDocumentLines(file, tagName);

        return await result.Match<Task<IActionResult>>(async content =>
        {
            await _songsCollection.InsertManyAsync(content);
            // await MergeSongs();
            return Ok(content.Count);
        },async request => await Task.FromResult(BadRequest(request)));
    }

    [HttpPost("sync-genres", Name = "SyncGenres")]
    public async Task<IActionResult> SyncGenres()
    {
        var songs = (await _songsCollection.FindAsync( x => x.GenreId == null)).ToList();
        for (var i = 0; i < songs.Count; i+=50)
        {
            var minimum = Math.Min(50, songs.Count - i);
            for (var j = i; j < i + minimum; j++)
            {
                var data = await _deezerClient.GetSongGenre(songs[j].Name, songs[j].Artist);
                await _songsCollection.UpdateOneAsync(x => x.Id == songs[j].Id, Builders<Song>.Update.Set(x => x.GenreId, data));
            }
        }

        return Ok();
    }
    
    [HttpGet("genres", Name = "GetGenres")]
    public async Task<ICollection<DeezerClient.Genre>> GetGenres([FromQuery] string? language)
    {
        return await _deezerClient.GetGenres(language);
    }

    private async Task MergeSongs()
    {
        var result = _songsCollection.Aggregate()
            .Group(
                x => new { x.Number, x.Artist, x.Name },
                x =>
                    new
                    {
                        x.Key,
                        Entries = x.Count()
                    })
            .ToList()
            .Where(x => x.Entries > 1)
            .ToList();
        foreach (var x1 in result)
        {
            var duplicateSongs = (await _songsCollection.FindAsync(
                x => x.Number == x1.Key.Number)).ToList();
            var idsToDelete = new List<string>();
            for (var i = 1; i < duplicateSongs.Count; i++)
            {
                duplicateSongs[0].Catalogs.AddRange(duplicateSongs[i].Catalogs);
                duplicateSongs[0].GenreId ??= duplicateSongs[i].GenreId;
                idsToDelete.Add(duplicateSongs[i].Id);
            }

            duplicateSongs[0].Catalogs = duplicateSongs[0].Catalogs.Distinct().ToList();
            await _songsCollection.FindOneAndReplaceAsync(
                x => x.Id == duplicateSongs[0].Id,
                duplicateSongs[0]);
            await _songsCollection.DeleteManyAsync(x => idsToDelete.Contains(x.Id));
        }
    }

    private static OneOf<ICollection<Song>, RequestError> ParseDocumentLines(IFormFile file, string catalogName = "")
    {
        if (!file.FileName.EndsWith("docx"))
        {
            return new RequestError($"Document extension should be docx - received file with name {file.FileName}");
        }

        using var doc = WordprocessingDocument.Open(file.OpenReadStream(), false);
        // Find the first table in the document.   
        var tables = doc.MainDocumentPart.Document.Body.Elements<Table>();

        // To get all rows from table
        var content = new Dictionary<string, Song>();
        var faultyLines = new List<string>();
        foreach (var table in tables)
        {
            var rows = table.Elements<TableRow>();
            // To read data from rows and to add records to the temporary table
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
                    var categoryName = category is "Title" or "Titre" ? "General" : category;
                    var song = new Song
                    {
                        Artist = currentRow[2].Trim(),
                        Name = currentRow[1].Trim(),
                        Number = int.Parse(currentRow[0]),
                        Categories = new List<string>{ categoryName},
                        Catalogs = new List<string> { catalogName}
                    };
                    if (content.ContainsKey(song.Key))
                    {
                        content[song.Key].Categories.Add(categoryName);
                        continue;
                    }
                    content.Add(song.Key, song);
                }
                catch (Exception)
                {
                    faultyLines.Add(string.Join(',', currentRow));
                }
            }
        }
        return faultyLines.Any() ? new RequestError("The lines in content were faulty", faultyLines) : content.Values.ToList();
    }
}