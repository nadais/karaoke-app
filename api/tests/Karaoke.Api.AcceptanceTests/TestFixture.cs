using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Model.Common;
using Ductus.FluentDocker.Services;

namespace Karaoke.Api.AcceptanceTests;

public class TestFixture : IDisposable
{
    public void Dispose()
    {
        _svc.Stop();
        _svc.Dispose();
    }

    private readonly ICompositeService _svc;
    private readonly CustomWebApplicationFactory _factory;

    public TestFixture()
    {
        var file = Path.Combine(Directory.GetCurrentDirectory(),
            (TemplateString) "docker-compose.yml");

        _svc = new Builder()
            .UseContainer()
            .UseCompose()
            .FromFile(file)
            .RemoveOrphans()
            .Build().Start();
        _factory = new CustomWebApplicationFactory();
    }

    public HttpClient GetClient()
    {
        return _factory.CreateDefaultClient();
    }
}