using Karaoke.Api.Data;
using MediatR;
using MongoDB.Driver;

namespace Karaoke.Api.Features.Songs;
public record Catalog(ICollection<Song> SongGroups);

public record GetSongsRequest : IRequest<Catalog>;
public class GetSongsHandler : IRequestHandler<GetSongsRequest, Catalog>
{
    private readonly IMongoCollection<Song> _songsCollection;
    
    public GetSongsHandler(MongoDbService mongoDbService)
    {
        _songsCollection = mongoDbService.GetSongsCollection();
    }

    public async Task<Catalog> Handle(GetSongsRequest request, CancellationToken cancellationToken)
    {
        var songs = await  _songsCollection.Find(_ => true).ToListAsync(cancellationToken: cancellationToken);
        return new Catalog(songs);
    }
}