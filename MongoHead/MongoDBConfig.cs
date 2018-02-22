using System;
using System.Collections.Generic;
using System.Text;

namespace MongoHead
{
    public class MongoDBConfig
    {
        public string ConnectionString { get; set; }
        public string DBName { get; set; }

        public MongoDBConfig(string ConnectionString, string DBName)
        {
            this.ConnectionString = ConnectionString;
            this.DBName = DBName;
        }
    }
}
