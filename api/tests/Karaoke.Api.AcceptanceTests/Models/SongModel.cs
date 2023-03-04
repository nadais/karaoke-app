using Karaoke.Api.Data;

namespace Karaoke.Api.AcceptanceTests.Models;

public class SongModel
{
    public string? Artist { get; set; }
    public string? Name { get; set; }
    public int Number { get; set; }
    
    public string? Catalog { get; set; }

    public Song ToSong()
    {
        return new Song
        {
            Artist = Artist ?? string.Empty,
            Number = Number,
            Name = Name ?? string.Empty,
            Catalogs = string.IsNullOrWhiteSpace(Catalog) ? new List<string>() : new List<string> { Catalog }
        };
    }

}