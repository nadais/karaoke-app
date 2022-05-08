using Karaoke.Api.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Karaoke.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly SongsCollectionSettings _settings;
    private readonly IMongoCollection<User> _usersCollection;
    public UsersController(
        MongoDbService mongoDbService,
        IOptions<SongsCollectionSettings> settings)
    {
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
        _usersCollection = mongoDbService.GetUsersCollection();
    }

    [HttpGet("{username}/songs",Name = "GetUserSongs")]
    public async Task<List<Song>> GetSongs(string username)
    {
        username = Uri.UnescapeDataString(username.ToLowerInvariant());
        var users = await  _usersCollection.Find(user => user.Name == username).ToListAsync();
        return users.First().Songs;
    }

    [HttpPost("clear")]
    public async Task ClearCache()
    {
        await _usersCollection.Database.DropCollectionAsync(_settings.UsersCollectionName);
        await _usersCollection.Database.CreateCollectionAsync(_settings.UsersCollectionName);
    }

    public record UserSongsRequest(List<SongRequest> Songs);
    [HttpPut("{username}/songs",Name = "UpdateSongs")]
    public async Task<IActionResult> Put(string username, [FromBody] UserSongsRequest request)
    {
        username = Uri.UnescapeDataString(username.Trim().ToLowerInvariant());
        var updateDefinition = Builders<User>.Update.Set(x => x.Songs, request.Songs.Select(x => new Song
        {
            Artist = x.Artist,
            Name = x.Name,
            Number = x.Number
        }));
        var users = await _usersCollection.FindOneAndUpdateAsync(x => x.Name == username, updateDefinition);
        if (users == null)
        {
            await _usersCollection.InsertOneAsync(new User
            {
                Name = username,
                Songs = request.Songs.Select(x => new Song
                {
                    Artist = x.Artist,
                    Name = x.Name,
                    Number = x.Number
                }).ToList()
            });
        }
        return Ok();
    }
}