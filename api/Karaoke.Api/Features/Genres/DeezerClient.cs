using System.Text.Json;

namespace Karaoke.Api.Features.Genres;

public record Genre(int Id, string Name, string Picture);

public class DeezerClient
{
    private readonly HttpClient _httpClient;
    private const string RemoteServiceBaseUrl = "https://api.deezer.com/";

    public DeezerClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public record DeezerSearchResult<TItem>(ICollection<TItem> Data);

    private record Album(int Id, string Title, int GenreId, Artist Artist);

    private record Artist(int Id, string Name);
    
    public async Task<ICollection<int>> GetSongGenre(string artistName)
    {
        var result = new HashSet<int>();
        try
        {
            var apiResult = await _httpClient.GetStringAsync($"{RemoteServiceBaseUrl}search/album?q=artist:\"{artistName.ToLowerInvariant()}\"");
            if (string.IsNullOrWhiteSpace(apiResult))
            {
                return result;
            }

            var search = JsonSerializer.Deserialize<DeezerSearchResult<Album>>(apiResult, new JsonSerializerOptions
            {
                PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance
            });
            if (search == null)
            {
                return result;
            }
            foreach (var deezerSong in search.Data)
            {
                if (string.Equals(deezerSong.Artist.Name, artistName, StringComparison.InvariantCultureIgnoreCase))
                {
                    result.Add(deezerSong.GenreId);
                }
            }

            return result;
        }
        catch (Exception)
        {
            return result;
        }
    }
    
    public async Task<ICollection<Genre>> GetGenres(string? language)
    {
        _httpClient.DefaultRequestHeaders.Add("Accept-Language", string.IsNullOrWhiteSpace(language) ? "en" : language);
        var apiResult = await _httpClient.GetFromJsonAsync<DeezerSearchResult<Genre>>(
            $"{RemoteServiceBaseUrl}genre");
        return apiResult?.Data ?? new List<Genre>();
    }
}