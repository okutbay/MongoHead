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
        public DateTime _DateCreated { get; set; }
        public DateTime _DateModified { get; set; }
    }
}
