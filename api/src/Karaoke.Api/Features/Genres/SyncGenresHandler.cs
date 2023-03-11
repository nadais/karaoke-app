using Karaoke.Api.Data;
using MediatR;
using MongoDB.Driver;

namespace Karaoke.Api.Features.Genres;

public record SyncGenresRequest : IRequest;
public class SyncGenresHandler : IRequestHandler<SyncGenresRequest>
{
    private readonly DeezerClient _deezerClient;
    private readonly IMongoCollection<Song> _songsCollection;

    public SyncGenresHandler(DeezerClient deezerClient, MongoDbService mongoDbService)
    {
        _deezerClient = deezerClient;
        _songsCollection = mongoDbService.GetSongsCollection();
    }

    public async Task Handle(SyncGenresRequest request, CancellationToken cancellationToken)
    {
        var artists = (await _songsCollection.FindAsync( x => x.Genres == null || x.Genres.Count == 0, cancellationToken: cancellationToken)).ToList(cancellationToken: cancellationToken)
            .Select(x => x.Artist)
            .Distinct()
            .ToList();
        foreach (var artist in artists)
        {
            var result = await _deezerClient.GetSongGenre(artist);
            await _songsCollection.UpdateManyAsync(x => x.Artist == artist, 
                Builders<Song>.Update.Set(x => x.Genres, result.ToList()), cancellationToken: cancellationToken);
        }
    }
}