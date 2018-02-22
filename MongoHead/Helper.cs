using System;
using System.Collections.Generic;
using System.Text;

using MongoDB.Bson;
using MongoDB.Driver;

using System.Linq.Expressions;


namespace MongoHead
{
    public static class Helper
    {
        public static IMongoDatabase GetDBInstance(MongoDBConfig MongoDBConfig)
        {
            //TODO: we need a way to store and pass the config to the layer and then helper class with a .net core way

            string connectionString = string.Empty;
            string dbName = string.Empty;

            connectionString = MongoDBConfig.ConnectionString;
            dbName = MongoDBConfig.DBName;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("MongoHead.Helper config error: invalid or undefined connection string setting");
            }

            if (string.IsNullOrEmpty(dbName))
            {
                throw new Exception("MongoHead.Helper config error: invalid or undefined dbname setting.");
            }

            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase db = client.GetDatabase(dbName);

            return db;
        }




    }

    /// <summary>
    /// just a dummy class to see class interface
    /// </summary>
    public class SomeClass
    {

        public void SomeMethod()
        {
            string connectionString = "";
            string dbname = "";

            MongoDBConfig mongoDBConfig = new MongoDBConfig(connectionString, dbname);

            IMongoDatabase db = Helper.GetDBInstance(mongoDBConfig);





        }
    }
}
