using Karaoke.Api.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Karaoke.Api;

public class MongoDbService
{
    private readonly SongsCollectionSettings _settings;

    public MongoDbService(IOptions<SongsCollectionSettings> settings)
    {
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
    }

    public async Task AddCollectionsIfNotExistsAsync()
    {
        var mongoDatabase = GetDatabase();
        var filter = new BsonDocument("name", _settings.SongsCollectionName);
        var options = new ListCollectionNamesOptions { Filter = filter };
        var hasCollection = await (await mongoDatabase.ListCollectionNamesAsync(options)).AnyAsync();
        if (!hasCollection)
        {
            await mongoDatabase.CreateCollectionAsync(_settings.SongsCollectionName);
        }
        filter = new BsonDocument("name", _settings.UsersCollectionName);
        options = new ListCollectionNamesOptions { Filter = filter };
        hasCollection = await (await mongoDatabase.ListCollectionNamesAsync(options)).AnyAsync();
        if (!hasCollection)
        {
            await mongoDatabase.CreateCollectionAsync(_settings.UsersCollectionName);
        }
    }
    
    public IMongoCollection<Song> GetSongsCollection()
    {
        var db = GetDatabase();
        return db.GetCollection<Song>(
            _settings.SongsCollectionName);
    }
    
    public IMongoCollection<User> GetUsersCollection()
    {
        var db = GetDatabase();
        return db.GetCollection<User>(
            _settings.UsersCollectionName);
    }

    private IMongoDatabase GetDatabase()
    {
        var mongoClientSettings = MongoClientSettings.FromConnectionString(_settings.GetFullConnectionString());
        mongoClientSettings.ServerApi = new ServerApi(ServerApiVersion.V1);
        var client = new MongoClient(mongoClientSettings);

        var mongoDatabase = client.GetDatabase(
            _settings.DatabaseName);
        return mongoDatabase;
    }
}