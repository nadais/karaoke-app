using System.Text.Json;

namespace Karaoke.Api;

public class DeezerClient
{
    private readonly HttpClient _httpClient;
    private const string _remoteServiceBaseUrl = "https://api.deezer.com/";

    public DeezerClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public record DeezerSearchResult<TItem>(ICollection<TItem> Data);
    public record Album(int Id, string Title, int GenreId, Artist Artist);

    public record Artist(int Id, string Name);
    
    public record Genre(int Id, string Name, string Picture);

    public async Task<int?> GetSongGenre(string songName, string artistName)
    {
        try
        {
            var apiResult = await _httpClient.GetStringAsync($"{_remoteServiceBaseUrl}search/album?strict=on&q=artist:\"{artistName.ToLowerInvariant()}\", track:\"{songName.ToLowerInvariant()}\"");
            if (apiResult == null)
            {
                return null;
            }

            var search = JsonSerializer.Deserialize<DeezerSearchResult<Album>>(apiResult, new JsonSerializerOptions
            {
                PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance
            });
            foreach (var deezerSong in search.Data)
            {
                if (string.Equals(deezerSong.Artist.Name, artistName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return deezerSong.GenreId;
                }
            }

            return -1;
        }
        catch (Exception ex)
        {
            return -1;
        }
    }
    
    public async Task<ICollection<Genre>> GetGenres(string? language)
    {
        _httpClient.DefaultRequestHeaders.Add("Accept-Language", string.IsNullOrWhiteSpace(language) ? "en" : language);
        var apiResult = await _httpClient.GetFromJsonAsync<DeezerSearchResult<Genre>>(
            $"{_remoteServiceBaseUrl}genre");
        return apiResult?.Data ?? new List<Genre>();
    }
}