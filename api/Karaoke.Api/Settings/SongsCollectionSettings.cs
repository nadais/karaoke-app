namespace Karaoke.Api.Settings;

public record SongsCollectionSettings
{
    public string ConnectionString { get; init; } = string.Empty;

    public string? DatabaseName { get; init; }
    
    public string? Username { get; init; }
    
    public string? Password { get; init; }

    public string? SongsCollectionName { get; set; }
    
    public string? UsersCollectionName { get; set; }

    public string GetFullConnectionString()
    {
        var connectionString = ConnectionString.Replace("<username>", Username)
            .Replace("<password>", Password);
        Console.WriteLine(connectionString);
        return connectionString;
    }
}