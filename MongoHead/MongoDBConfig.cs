using System;
using System.Collections.Generic;
using System.Text;

namespace MongoHead
{
    public class MongoDBConfig
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

        public MongoDBConfig(string ConnectionString, string DatabaseName)
        {
            this.ConnectionString = ConnectionString;
            this.DatabaseName = DatabaseName;
        }
    }
}
