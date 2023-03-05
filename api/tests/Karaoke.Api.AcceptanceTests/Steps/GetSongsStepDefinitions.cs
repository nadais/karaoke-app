using BoDi;
using FluentAssertions;
using Karaoke.Api.AcceptanceTests.Extensions;
using Karaoke.Api.AcceptanceTests.Models;
using Karaoke.Api.Data;
using Karaoke.Api.Features.Songs;
using MongoDB.Driver;
using TechTalk.SpecFlow.Assist;

namespace Karaoke.Api.AcceptanceTests.Steps;

[Binding]
public class GetSongsStepDefinitions
{
    private readonly IMongoCollection<Song> _songsCollection;
    private readonly HttpClient _client;
    private readonly ScenarioContext _scenarioContext;

    public GetSongsStepDefinitions(IObjectContainer container, ScenarioContext scenarioContext)
    {
        _songsCollection = container.Resolve<IMongoCollection<Song>>();
        _client = container.Resolve<HttpClient>(Hooks.Hooks.ClientName);
        _scenarioContext = scenarioContext;
    }

    [Given(@"the following songs are loaded")]
    public async Task GivenTheFollowingSongsAreLoaded(Table table)
    {
        var songs = table.CreateSet<SongModel>();
        await _songsCollection.InsertManyAsync(songs.Select(x => x.ToSong()).ToList());
    }

    [When(@"I send a get songs request")]
    public async Task WhenISendAGetSongsRequest()
    {
        var response = await _client.GetAsync("/songs");
        _scenarioContext.SetResponse(response);
    }

    [Then(@"I should have (.*) songs in response for catalog '(.*)'")]
    public async Task ThenIShouldHaveSongsInResponse(int songLength, string catalogName)
    {
        var response = _scenarioContext.GetResponse();
        var catalog = await response.ParseAs<Catalog>();
        var songsInCatalog = catalog.SongGroups.Count(x => x.Catalogs.Contains(catalogName));
        songsInCatalog.Should().Be(songLength);
    }
}