namespace Karaoke.Api.AcceptanceTests.Extensions;

public static class ScenarioContextExtensions
{
    public static HttpResponseMessage GetResponse(this ScenarioContext context)
    {
        var result = context["result"] as HttpResponseMessage;
        ArgumentNullException.ThrowIfNull(result);
        return result;
    }
    public static void SetResponse(this ScenarioContext context, HttpResponseMessage response)
    {
        context["result"] = response;
    }
}