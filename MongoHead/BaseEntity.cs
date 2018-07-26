using System;
using System.Collections.Generic;
using System.Text;

using MongoDB.Bson;

namespace MongoHead
{
    public class BaseEntity : BaseEntitySimple
    {
        public bool _IsActive { get; set; }
        public DateTime _DateUtcModified { get; set; }
    }

    public class BaseEntitySimple
    {
        public ObjectId _id { get; set; }
        public DateTime _DateUtcCreated { get; set; }
    }
}
