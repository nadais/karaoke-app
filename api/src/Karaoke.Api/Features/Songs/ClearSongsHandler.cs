using Karaoke.Api.Data;
using MediatR;
using MongoDB.Driver;

namespace Karaoke.Api.Features.Songs;

public record ClearSongsRequest(string TagName) :IRequest; 
public class ClearSongsHandler : IRequestHandler<ClearSongsRequest>
{
    private readonly IMongoCollection<Song> _songsCollection;
    public ClearSongsHandler(
        MongoDbService mongoDbService)
    {
        _songsCollection = mongoDbService.GetSongsCollection();
    }
    public async Task Handle(ClearSongsRequest request, CancellationToken cancellationToken)
    {
        await _songsCollection.DeleteManyAsync(x => x.Catalogs.Contains(request.TagName), cancellationToken: cancellationToken);
    }
}