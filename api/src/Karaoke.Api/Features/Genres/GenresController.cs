using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Karaoke.Api.Features.Genres;

[Route("[controller]")]
public class GenresController : ControllerBase
{
    private readonly DeezerClient _deezerClient;
    private readonly IMediator _mediator;
    public GenresController(
        DeezerClient deezerClient,
        IMediator mediator)
    {
        _deezerClient = deezerClient;
        _mediator = mediator;
    }

    [HttpPost("sync", Name = "SyncGenres")]
    public async Task<IActionResult> SyncGenres()
    {
        await _mediator.Send(new SyncGenresRequest());
        return Ok();
    }
    
    [HttpGet]
    public async Task<ICollection<Genre>> GetGenres([FromQuery] string? language)
    {
        return await _deezerClient.GetGenres(language);
    }

}