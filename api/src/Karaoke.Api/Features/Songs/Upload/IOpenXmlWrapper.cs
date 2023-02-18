using OneOf;

namespace Karaoke.Api.Features.Songs.Upload;


public record InternalTable(List<string> Headers, List<List<string>> Content);

public interface IOpenXmlWrapper
{
    public OneOf<List<InternalTable>, RequestError> ParseLines(IFormFile file);
}
