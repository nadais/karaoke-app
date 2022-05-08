namespace Karaoke.Api;

public record SongRequest
{
    public string Artist { get; set; }
    public string Name { get; set; }
    public int Number { get; set; }
}