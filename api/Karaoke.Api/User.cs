using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Karaoke.Api;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Name { get; set; }

    public List<Song> Songs { get; set; }
}