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

    public async Task<ICollection<int>> GetSongGenre(string artistName)
    {
        var result = new HashSet<int>();
        try
        {
            var apiResult = await _httpClient.GetStringAsync($"{_remoteServiceBaseUrl}search/album?q=artist:\"{artistName.ToLowerInvariant()}\"");
            if (apiResult == null)
            {
                return result;
            }

            var search = JsonSerializer.Deserialize<DeezerSearchResult<Album>>(apiResult, new JsonSerializerOptions
            {
                PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance
            });
            foreach (var deezerSong in search.Data)
            {
                if (string.Equals(deezerSong.Artist.Name, artistName, StringComparison.InvariantCultureIgnoreCase))
                {
                    result.Add(deezerSong.GenreId);
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            return result;
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