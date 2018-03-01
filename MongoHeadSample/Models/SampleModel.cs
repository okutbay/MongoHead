using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace MongoHeadSample.Models
{
    public class SampleModel
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string FriendlyId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime ModifyDate { get; set; } = DateTime.Now;
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public int UserId { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
