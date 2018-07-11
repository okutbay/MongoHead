using System;
using System.Collections.Generic;
using System.Text;

using MongoDB.Bson;
using MongoDB.Driver;

using System.Linq.Expressions;

namespace MongoHead
{
    public class Helper
    {
        //MongoClient _client;
        ////MongoServer _server;
        //IMongoDatabase _db;

        private MongoDBConfig _mongoDBConfig { get; set; }

        //These constants are used to access specic properties of the entity to access them in run time for insert or update issues
        private const string keyFieldNameConst = "_id";
        private const string dateCreatedFieldNameConst = "DateCreated";
        private const string dateModifiedFieldNameConst = "DateModified";

        public Helper(MongoDBConfig MongoDBConfig)
        {
            this._mongoDBConfig = MongoDBConfig;
        }

        public IMongoDatabase GetDBInstance(string DBName)
        {
            //TODO: we need a way to store and pass the config to the layer and then helper class with a .net core way

            string connectionString = string.Empty;
            string dbName = string.Empty;

            connectionString = this._mongoDBConfig.ConnectionString;
            
            //if no specific database name is given operations will be carried on default database 
            if (!string.IsNullOrEmpty(DBName))
            {
                dbName = DBName;
            }
            else
            {
                dbName = this._mongoDBConfig.DefaultDatabaseName;
            }


            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("MongoHead.Helper config error: invalid or undefined connection string setting");
            }

            if (string.IsNullOrEmpty(dbName))
            {
                throw new Exception("MongoHead.Helper config error: invalid or undefined dbname setting.");
            }

            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase _db = client.GetDatabase(dbName);//we are not storing the dbname to instance. Every helper method can work on different databases

            return _db;
        }

        //SAVE DATA METHODS **************************************************************
        #region Save

        public ObjectId Save(string CollectionName, object ObjectToSave, string dbName = "")
        {
            IMongoDatabase db = this.GetDBInstance(dbName);
            IMongoCollection<BsonDocument> collection = db.GetCollection<BsonDocument>(CollectionName);

            BsonDocument document = ObjectToSave.ToBsonDocument(); //this adds _t as type of the object to the 
            document.Remove("_t"); //we dont want this just remove

            ObjectId newId = this.Save(CollectionName, document, dbName);
            return newId;
        }

        public ObjectId Save(string CollectionName, BsonDocument ObjectToSave, string dbName = "")
        {
            IMongoDatabase db = this.GetDBInstance(dbName);
            IMongoCollection<BsonDocument> collection = db.GetCollection<BsonDocument>(CollectionName);

            collection.InsertOne(ObjectToSave);

            string id = ObjectToSave[keyFieldNameConst].ToString();

            ObjectId newId = new ObjectId(id);
            return newId;
        } 

        #endregion





        public static List<T> GetAll<T>(string CollectionName, string dbName = "")
        {
            //IMongoDatabase db = Helper.GetDBInstance(dbName);

            //IMongoCollection<T> collection = db.GetCollection<T>(CollectionName);

            List<T> list = new List<T>();
            //foreach (var item in collection.Find<T>().ToList<T>())
            //{
            //    list.Add(item);
            //}

            return list;
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

            IMongoDatabase db = new Helper(mongoDBConfig).GetDBInstance(dbname);
        }
    }
}
