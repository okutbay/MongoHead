using System;
using System.Collections.Generic;
using System.Text;

using MongoDB.Bson;

namespace MongoHead
{
    public class BaseEntity
    {
        public ObjectId _id { get; set; }
        public bool _IsActive { get; set; }
        public DateTime _DateUtcCreated { get; set; }
        public DateTime _DateUtcModified { get; set; }
        public TimeSpan _DateUtcOffset { get; set; }
    }
}
