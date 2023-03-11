using BoDi;
using Karaoke.Api.AcceptanceTests.Extensions;

namespace Karaoke.Api.AcceptanceTests.Steps;

[Binding]
public class DeleteCatalogStepDefinitions
{
    private readonly HttpClient _client;
    private readonly ScenarioContext _scenarioContext;

    public DeleteCatalogStepDefinitions(IObjectContainer container, ScenarioContext scenarioContext)
    {
        _client = container.Resolve<HttpClient>(Hooks.Hooks.ClientName);
        _scenarioContext = scenarioContext;
    }

    [When(@"I send a delete songs request for (.*) catalog")]
    public async Task WhenISendADeleteSongsRequestForCatalog(string catalogName)
    {
        var response = await _client.DeleteAsync($"/songs/{catalogName}");
        _scenarioContext.SetResponse(response);
    }
}