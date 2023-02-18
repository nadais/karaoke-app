using Karaoke.Api.Data;
using OneOf;

namespace Karaoke.Api.Features.Songs.Upload;

public interface ISongsDocumentParser
{
    public OneOf<ICollection<Song>, RequestError> ParseDocumentLines(List<InternalTable> tables, string catalogName);
}