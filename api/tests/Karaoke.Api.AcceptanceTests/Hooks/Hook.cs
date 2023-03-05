using BoDi;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Model.Common;
using Ductus.FluentDocker.Services;
using Karaoke.Api.Data;
using Karaoke.Api.Features.Songs;
using MongoDB.Driver;

namespace Karaoke.Api.AcceptanceTests.Hooks;

[Binding]
public class Hooks
{
    public const string ClientName = "ApiClient";
    private static ICompositeService? _svc;
    private readonly IObjectContainer _objectContainer;

    public Hooks(IObjectContainer objectContainer)
    {
        _objectContainer = objectContainer;
    }

    [BeforeTestRun]
    public static void SetupDocker()
    {
        var file = Path.Combine(Directory.GetCurrentDirectory(),
            (TemplateString)"docker-compose.yml");

        _svc = new Builder()
            .UseContainer()
            .UseCompose()
            .FromFile(file)
            .RemoveOrphans()
            .Build().Start();
    }

    [AfterTestRun]
    public static void ClearDocker()
    {
        _svc?.Stop();
    }

    [BeforeScenario]
    public async Task BeforeScenario()
    {
        SetupHttpClient();
        RegisterSongsCollection();
        await PurgeSongsCollectionAsync();
    }
        
    [AfterScenario]
    public async Task AfterScenario()
    {
        await PurgeSongsCollectionAsync();
    }

    private void SetupHttpClient()
    {
        var factory = new CustomWebApplicationFactory();
        var client = factory.CreateDefaultClient();
        _objectContainer.RegisterInstanceAs(client, ClientName);
    }

    private void RegisterSongsCollection()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
            .Build();
        var settings = new SongsCollectionSettings();
        config.Bind(SongsCollectionSettings.KeyName, settings);
        var db = GetDatabase(settings);
        var songsCollection = db.GetCollection<Song>(
            settings.SongsCollectionName);
        _objectContainer.RegisterInstanceAs(songsCollection);
    }

    private async Task PurgeSongsCollectionAsync()
    {
        var songsCollection = _objectContainer.Resolve<IMongoCollection<Song>>();
        await songsCollection.DeleteManyAsync(_ => true);
            
    }

    private static IMongoDatabase GetDatabase(SongsCollectionSettings settings)
    {
        var mongoClientSettings = MongoClientSettings.FromConnectionString(settings.GetFullConnectionString());
        mongoClientSettings.ServerApi = new ServerApi(ServerApiVersion.V1);
        var client = new MongoClient(mongoClientSettings);

        var mongoDatabase = client.GetDatabase(
            settings.DatabaseName);
        return mongoDatabase;
    }
}