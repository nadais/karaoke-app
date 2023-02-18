using System.Net;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace Karaoke.Api.AcceptanceTests.Steps;

[Binding]
public sealed class UploadSongsStepDefinitions : IClassFixture<TestFixture>
{
    // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

    private readonly ScenarioContext _scenarioContext;
    private readonly TestFixture _fixture;
    private HttpResponseMessage _response;

    public UploadSongsStepDefinitions(ScenarioContext scenarioContext, TestFixture fixture)
    {
        _fixture = fixture;
        _scenarioContext = scenarioContext;
    }

    [When("I send an upload catalog request")]
    public async Task WhenISendAnUploadCatalogRequest()
    {
        var client = _fixture.GetClient();
        using var stream = await GetStreamAsync();
        MultipartFormDataContent fileContent = new();
        fileContent.Add(new StreamContent(stream), "file", "catalog.docx");
        var result = await client.PutAsync("/songs/something", fileContent);
        _scenarioContext["result"] = result;
    }
    
    [Then("I should receive a successful response")]
    public void ThenIShouldReceiveASuccessfulResponse()
    {
        _response = _scenarioContext["result"] as HttpResponseMessage;
        _response.Should().NotBeNull();
        _response!.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Then("I should get a positive number of songs inserted")]
    public async Task ThenIShouldGetAPositiveNumberOfSongsInserted()
    {
        var content = await _response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<int>(content);
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