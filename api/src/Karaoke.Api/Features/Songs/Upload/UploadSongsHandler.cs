using Karaoke.Api.Data;
using MediatR;
using MongoDB.Driver;
using OneOf;

namespace Karaoke.Api.Features.Songs.Upload;

public record UploadSongsRequest(string TagName, IFormFile File) : IRequest<OneOf<int, RequestError>>;
public record RequestError(string? Message = null, object? Content = null);

public class UploadSongsHandler : IRequestHandler<UploadSongsRequest, OneOf<int, RequestError>>
{
    private readonly IMongoCollection<Song> _songsCollection;
    private readonly IOpenXmlWrapper _openXmlWrapper;
    private readonly ISongsDocumentParser _songsDocumentParser;
    public UploadSongsHandler(MongoDbService mongoDbService, ISongsDocumentParser songsDocumentParser, IOpenXmlWrapper openXmlWrapper)
    {
        _songsDocumentParser = songsDocumentParser;
        _openXmlWrapper = openXmlWrapper;
        _songsCollection = mongoDbService.GetSongsCollection();
    }

    public async Task<OneOf<int, RequestError>> Handle(UploadSongsRequest request, CancellationToken cancellationToken)
    {
        var data = _openXmlWrapper.ParseLines(request.File);
        if (data.IsT1)
        {
            return data.AsT1;
        }
        var result = _songsDocumentParser.ParseDocumentLines(data.AsT0, request.TagName);
        if (result.IsT1)
        {
            return result.AsT1;
        }
        await _songsCollection.InsertManyAsync(result.AsT0, cancellationToken: cancellationToken);
        return result.AsT0.Count;
    }
}