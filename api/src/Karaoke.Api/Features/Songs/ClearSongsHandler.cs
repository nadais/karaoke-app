using Karaoke.Api.Data;
using MediatR;
using MongoDB.Driver;
using OneOf;
using OneOf.Types;

namespace Karaoke.Api.Features.Songs;

public record ClearSongsRequest(string TagName) :IRequest<OneOf<Success, NotFound>>; 
public class ClearSongsHandler : IRequestHandler<ClearSongsRequest, OneOf<Success, NotFound>>
{
    private readonly IMongoCollection<Song> _songsCollection;
    public ClearSongsHandler(
        MongoDbService mongoDbService)
    {
        _songsCollection = mongoDbService.GetSongsCollection();
    }
    public async Task<OneOf<Success, NotFound>> Handle(ClearSongsRequest request, CancellationToken cancellationToken)
    {
        var result = await _songsCollection.DeleteManyAsync(x => x.Catalogs.Contains(request.TagName), cancellationToken: cancellationToken);
        if (result.DeletedCount == 0)
        {
            return new NotFound();
        }

        return new Success();
    }
}