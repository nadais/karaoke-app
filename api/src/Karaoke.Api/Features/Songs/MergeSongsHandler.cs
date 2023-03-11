using Karaoke.Api.Data;
using MediatR;
using MongoDB.Driver;

namespace Karaoke.Api.Features.Songs;

public record MergeSongsRequest : IRequest;

public class MergeSongsHandler : IRequestHandler<MergeSongsRequest>
{
    private readonly IMongoCollection<Song> _songsCollection;

    public MergeSongsHandler(MongoDbService mongoDbService)
    {
        _songsCollection = mongoDbService.GetSongsCollection();
    }

    public async Task Handle(MergeSongsRequest request, CancellationToken cancellationToken)
    {
        var result = _songsCollection.Aggregate()
            .Group(
                x => new { x.Number, x.Artist, x.Name },
                x =>
                    new
                    {
                        x.Key,
                        Entries = x.Count()
                    })
            .ToList(cancellationToken: cancellationToken)
            .Where(x => x.Entries > 1)
            .ToList();
        foreach (var x1 in result)
        {
            var duplicateSongs = (await _songsCollection.FindAsync(
                x => x.Number == x1.Key.Number, cancellationToken: cancellationToken))
                .ToList(cancellationToken: cancellationToken);
            var idsToDelete = new List<string>();
            for (var i = 1; i < duplicateSongs.Count; i++)
            {
                duplicateSongs[0].Catalogs.AddRange(duplicateSongs[i].Catalogs);
                duplicateSongs[0].Genres ??= duplicateSongs[i].Genres;
                idsToDelete.Add(duplicateSongs[i].Id);
            }

            duplicateSongs[0].Catalogs = duplicateSongs[0].Catalogs.Distinct().ToList();
            await _songsCollection.FindOneAndReplaceAsync(
                x => x.Id == duplicateSongs[0].Id,
                duplicateSongs[0], cancellationToken: cancellationToken);
            await _songsCollection.DeleteManyAsync(x => idsToDelete.Contains(x.Id),
                cancellationToken: cancellationToken);
        }

    }
}