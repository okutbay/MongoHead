using System;
using System.Collections.Generic;
using System.Text;

using MongoDB.Bson;

namespace MongoHead
{
    public class BaseEntityComplex : BaseEntity
    {
        public int _UserId { get; set; }
    }

    public class BaseEntity : BaseEntityLight
    {
        public bool _IsActive { get; set; }
        public DateTime _DateUtcModified { get; set; }
    }

    public class BaseEntityLight : BaseEntitySimple
    {
        public DateTime _DateUtcCreated { get; set; }
    }

    public class BaseEntitySimple 
    {
        public ObjectId _id { get; set; }
    }
}
