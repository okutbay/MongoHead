using System;
using System.Collections.Generic;
using System.Text;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoHeadSampleApp
{
    public class Product
    {
        public ObjectId Id { get; set; }
        [BsonElement("ProductId")]
        public int ProductId { get; set; }
        [BsonElement("ProductName")]
        public string ProductName { get; set; }
        [BsonElement("Price")]
        public int Price { get; set; }
        [BsonElement("Category")]
        public string Category { get; set; }
    }
}
