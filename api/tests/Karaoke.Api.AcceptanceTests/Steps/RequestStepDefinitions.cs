using System.Net;
using FluentAssertions;
using Karaoke.Api.AcceptanceTests.Extensions;

namespace Karaoke.Api.AcceptanceTests.Steps;

[Binding]
public class RequestStepDefinitions
{
    private readonly ScenarioContext _scenarioContext;

    public RequestStepDefinitions(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }
    [Then("I should receive a (.*) response")]
    public void ThenIShouldReceiveASuccessfulResponse(int statusCode)
    {
        var response = _scenarioContext.GetResponse();
        response.Should().NotBeNull();
        ((int)response.StatusCode).Should().Be(statusCode);
    }
}