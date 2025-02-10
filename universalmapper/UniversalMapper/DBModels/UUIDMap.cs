using OrangeButton.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UniversalMapper.DBModels;

public class UUIDMap
{
    public ObjectId Id { get; set; }

    [BsonRequired]
    public Guid UUID { get; set; }

    public AlternativeIdentifier AlternativeIdentifier { get; set; } = new AlternativeIdentifier();
}