using Karaoke.Api.Data;
using MediatR;
using MongoDB.Driver;

namespace Karaoke.Api.Features.Songs;

public record GetSongsStatusRequest: IRequest<StatusResponse>;
public record StatusResponse(List<string> Duplicates, List<string> AvailableNumbers);
public class GetSongsStatusHandler : IRequestHandler<GetSongsStatusRequest, StatusResponse>
{
    private readonly IMongoCollection<Song> _songsCollection;
    
    public GetSongsStatusHandler(MongoDbService mongoDbService)
    {
        _songsCollection = mongoDbService.GetSongsCollection();
    }
    
    public async Task<StatusResponse> Handle(GetSongsStatusRequest r, CancellationToken c)
    {
        var duplicateSongs = await GetDuplicateSongsAsync(c);
        var missingNumbers = await GetMissingNumbersAsync(c);
        return new StatusResponse(duplicateSongs, missingNumbers);
    }
    
    private async Task<List<string>> GetMissingNumbersAsync(CancellationToken cancellationToken)
    {
        var songNumbers = await _songsCollection.Find(_ => true)
            .Project(x => x.Number)
            .ToListAsync(cancellationToken: cancellationToken);
        songNumbers = songNumbers.OrderBy(x => x).ToList();
        var startNumber = 1;
        var missingNumbers = new List<string>();
        foreach (var songNumber in songNumbers)
        {
            startNumber = HandleSongNumber(startNumber, songNumber, missingNumbers);
        }

        missingNumbers.Add($"{startNumber}+");
        return missingNumbers;
    }

    private static int HandleSongNumber(int startNumber, int songNumber, ICollection<string> missingNumbers)
    {
        if (startNumber == songNumber)
        {
            startNumber++;
            return startNumber;
        }

        if (startNumber > songNumber)
        {
            return startNumber;
        }

        missingNumbers.Add(startNumber == songNumber - 1 ? $"{startNumber}" : $"{startNumber}-{songNumber - 1}");
        startNumber = songNumber + 1;
        return startNumber;
    }

    private async Task<List<string>> GetDuplicateSongsAsync(CancellationToken cancellationToken)
    {
        var result = GetNumbersWithMultipleSongs(cancellationToken);
        var songs = (await _songsCollection.FindAsync(x => result.Contains(x.Number), cancellationToken: cancellationToken))
            .ToList(cancellationToken: cancellationToken)
            .OrderBy(x => x.Number)
            .Select(x => $"{x.Number}_{x.Artist}_{x.Name}")
            .ToList();
        return songs;
    }

    private List<int> GetNumbersWithMultipleSongs(CancellationToken cancellationToken)
    {
        var result = _songsCollection.Aggregate()
            .Group(
                x => x.Number,
                x =>
                    new
                    {
                        x.Key,
                        Entries = x.Count()
                    })
            .ToList(cancellationToken: cancellationToken)
            .Where(x => x.Entries > 1)
            .Select(x => x.Key)
            .ToList();
        return result;
    }
}