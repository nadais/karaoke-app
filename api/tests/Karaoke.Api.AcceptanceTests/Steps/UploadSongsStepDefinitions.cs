using System.Text.Json;
using BoDi;
using FluentAssertions;
using Karaoke.Api.AcceptanceTests.Extensions;
using Xunit;

namespace Karaoke.Api.AcceptanceTests.Steps;

[Binding]
public sealed class UploadSongsStepDefinitions
{
    private readonly ScenarioContext _scenarioContext;
    private readonly HttpClient _client;

    public UploadSongsStepDefinitions(ScenarioContext scenarioContext, IObjectContainer container)
    {
        _client = container.Resolve<HttpClient>(Hooks.Hooks.ClientName);
        _scenarioContext = scenarioContext;
    }

    [When("I send an upload catalog request")]
    public async Task WhenISendAnUploadCatalogRequest()
    {
        using var stream = await GetStreamAsync();
        MultipartFormDataContent fileContent = new();
        fileContent.Add(new StreamContent(stream), "file", "catalog.docx");
        var response = await _client.PutAsync("/songs/something", fileContent);
        _scenarioContext.SetResponse(response);
    }
    
    [Then("I should get a positive number of songs inserted")]
    public async Task ThenIShouldGetAPositiveNumberOfSongsInserted()
    {
        var response = _scenarioContext.GetResponse();
        var result = await response.ParseAs<int>();
        result.Should().BeGreaterThan(0);
    }

    private static async Task<MemoryStream> GetStreamAsync()
    {
        var stream = new MemoryStream();
        await using var fileStream = File.OpenRead("catalog.docx");
        await fileStream.CopyToAsync(stream);
        fileStream.Close();
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }
}