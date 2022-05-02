using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Karaoke.Api;

public class Song
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Artist { get; set; }
    public string Name { get; set; }
    public int Number { get; set; }
    
    public int? GenreId { get; set; }

    public List<int>? Genres { get; set; }
    public List<string> Categories { get; set; }

    public string Key => $"{Number}_{Name}_{Artist}";

    public List<string> Catalogs { get; set; }
}