using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoHead;

/// <summary>
/// Basic entity model with only addition "_id" field.
/// Please note that inherited fields starts with underscore ("_")
/// </summary>
public class BaseEntitySimple
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]

    public ObjectId _id { get; set; }
}

/// <summary>
/// Basic with "_DateUtcCreated" addition. 
/// Your entities is going to have these fields if you inherit this class:
/// "_id", "_DateUtcCreated"
/// Please note that inherited fields starts with underscore ("_")
/// </summary>
public class BaseEntityLight : BaseEntitySimple
{
    public DateTime _DateUtcCreated { get; set; }
}

/// <summary>
/// Light with "_IsActive", "_DateUtcModified" additions.
/// Your entities is going to have these fields if you inherit this class:
/// "_id", "_DateUtcCreated", "_IsActive", "_DateUtcModified"
/// Please note that inherited fields starts with underscore ("_")
/// </summary>
public class BaseEntity : BaseEntityLight
{
    public bool _IsActive { get; set; }
    public DateTime _DateUtcModified { get; set; }
}

/// <summary>
/// This is the most complex structure for MongoHead BaseEntities
/// Your entities is going to have these fields if you inherit this class:
/// "_id", "_DateUtcCreated", "_IsActive", "_DateUtcModified", "_UserId"
/// Please note that inherited fields starts with underscore ("_")
/// </summary>
public class BaseEntityComplex : BaseEntity
{
    public string _UserId { get; set; }
}