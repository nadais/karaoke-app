using Karaoke.Api.Features.Songs.Upload;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Karaoke.Api.Features.Songs;

[ApiController]
[Route("[controller]")]
public class SongsController : ControllerBase
{
    private readonly IMediator _mediator;
    public SongsController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet(Name = "GetSongs")]
    public async Task<Catalog> GetSongs()
    {
        return await _mediator.Send(new GetSongsRequest());
    }
    
    [HttpGet("status",Name = "FindDuplicates")]
    public async Task<StatusResponse> GetDuplicates(CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetSongsStatusRequest(), cancellationToken);
    }
    
    [HttpPost("merge")]
    public async Task Merge()
    {
        await _mediator.Send(new MergeSongsRequest());
    }

    [HttpDelete("{tagName}")]
    public async Task ClearCache(string tagName)
    {
        tagName = Uri.UnescapeDataString(tagName);
        await _mediator.Send(new ClearSongsRequest(tagName));
    }

    [HttpPut("{tagName}", Name = "UploadList")]
    public async Task<IActionResult> Post(string tagName, [FromForm] IFormFile file)
    {
        tagName = Uri.UnescapeDataString(tagName);
        var result = await _mediator.Send(new UploadSongsRequest(tagName, file));

        return result.Match<IActionResult>(content => Ok(content), BadRequest);
    }
}