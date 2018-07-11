using System;
using System.Collections.Generic;
using System.Text;

namespace MongoHead
{
    public class MongoDBConfig
    {
        public string ConnectionString { get; set; }
        public string DefaultDatabaseName { get; set; }

        public MongoDBConfig(string ConnectionString, string DefaultDatabaseName)
        {
            this.ConnectionString = ConnectionString;
            this.DefaultDatabaseName = DefaultDatabaseName;
        }
    }
}
