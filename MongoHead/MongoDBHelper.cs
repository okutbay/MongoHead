using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoHead
{
    public class MongoDBHelper
    {
        readonly MongoDBConfig _Config;
        private IMongoDatabase Db { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CollectionName { get; set; }

        /// <summary>
        /// "IDFieldName" constant is used to access specific "_id" field property name of the base entity to access it in run-time for insert, update or delete purposes
        /// </summary>
        public string IDFieldName { get { return "_id"; } }

        /// <summary>
        /// "DateCreatedFieldName" constant is used to access specific "_DateCreated" field property name of the base entity to access it in run-time for insert, update or delete purposes
        /// </summary>
        public string DateUtcCreatedFieldName { get { return "_DateUtcCreated"; } }

        /// <summary>
        /// "DateModifiedFieldName" constant is used to access specific "_DateModified" field property name of the base entity to access it in run-time for insert, update or delete purposes
        /// </summary>
        public string DateUtcModifiedFieldName { get { return "_DateUtcModified"; } }

        /// <summary>
        /// "IsActiveFieldName" constant is used to access specific "_IsActive" field property name of the base entity to access it in run-time for insert, update or delete purposes
        /// </summary>
        public string IsActiveFieldName { get { return "_IsActive"; } }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="Config"></param>
        /// <param name="EntityType"></param>
        public MongoDBHelper(MongoDBConfig Config, Type EntityType)
        {
            _Config = Config;

            this.CollectionName = EntityType.Name;

            this.Db = this.GetDBInstance();
        }

        /// <summary>
        /// Returns an instance of Mongo Database with database name defined in the config object. 
        /// </summary>
        /// <returns></returns>
        private IMongoDatabase GetDBInstance()
        {
            string connectionString = string.Empty;
            string dbName = string.Empty;

            connectionString = this._Config.ConnectionString;
            dbName = this._Config.DatabaseName;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("MongoHead.MongoDBHelper config error: invalid or undefined connection string setting");
            }

            if (string.IsNullOrEmpty(dbName))
            {
                throw new Exception("MongoHead.MongoDBHelper config error: DBName is not set. Please check your DefaultDatabaseName setting or PreferredDBName");
            }

            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase _db = client.GetDatabase(dbName);

            return _db;
        }


        //SAVE DATA METHODS **************************************************************
        #region SAVE

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ObjectToSave"></param>
        /// <returns></returns>
        public ObjectId Save(object ObjectToSave)
        {
            IMongoCollection<BsonDocument> collection = Db.GetCollection<BsonDocument>(CollectionName);

            BsonDocument document = ObjectToSave.ToBsonDocument(); //conversion to BsonDocument adds _t as type of the object to the 
            document.Remove("_t"); //we dont want this just remove

            ObjectId newId = this.Save(document);
            return newId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="BsonDocumentToSave"></param>
        /// <returns></returns>
        public ObjectId Save(BsonDocument BsonDocumentToSave)
        {
            IMongoCollection<BsonDocument> collection = Db.GetCollection<BsonDocument>(CollectionName);

            collection.InsertOne(BsonDocumentToSave);

            string id = BsonDocumentToSave[IDFieldName].ToString();

            ObjectId newId = new ObjectId(id);
            return newId;
        }

        #endregion


        // GET LIST DATA METHODS **************************************************************
        #region GET LIST

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetList<T>()
        {
            //TODO bunun icinde asagidaki gibi bir cagriyla cozebilir miyiz test edelim
            //return GetList<T>(null);

            IMongoCollection<T> collection = Db.GetCollection<T>(CollectionName);

            var filter = new BsonDocument();

            List<T> list = new List<T>();
            foreach (var item in collection.Find(filter).ToEnumerable())
            {
                list.Add(item);
            }

            return list;

            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Filter"></param>
        /// <returns></returns>
        public List<T> GetList<T>(List<Filter> Filter)
        {
            return GetList<T>(Filter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Filter"></param>
        /// <param name="UseAndLogic"></param>
        /// <returns></returns>
        public List<T> GetList<T>(List<Filter> Filter, bool UseAndLogic = true)
        {
            IMongoCollection<T> collection = Db.GetCollection<T>(CollectionName);

            var exp = ExpressionBuilder.GetExpression<T>(Filter, UseAndLogic);
            var query = Builders<T>.Filter.Where(exp);

            IEnumerable<T> cursor = collection.Find(query).ToEnumerable();

            List<T> list = new List<T>();
            foreach (var item in cursor)
            {
                list.Add(item);
            }

            return list;
        }

        #endregion


        // GET DATA METHODS **************************************************************
        #region GET

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Filter"></param>
        /// <returns></returns>
        public T Get<T>(List<Filter> Filter)
        {
            return Get<T>(Filter, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Filter"></param>
        /// <param name="UseAndLogic"></param>
        /// <returns></returns>
        public T Get<T>(List<Filter> Filter, bool UseAndLogic = true)
        {
            IMongoCollection<T> collection = Db.GetCollection<T>(CollectionName);

            var exp = ExpressionBuilder.GetExpression<T>(Filter, UseAndLogic);
            var query = Builders<T>.Filter.Where(exp);

            var foundItem = collection.Find(query).FirstOrDefault();

            return foundItem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_id"></param>
        /// <returns></returns>
        public T GetByObjectId<T>(ObjectId _id)
        {
            IMongoCollection<T> collection = Db.GetCollection<T>(CollectionName);

            var itemParameter = Expression.Parameter(typeof(T), "item");
            var whereExpression = Expression.Lambda<Func<T, bool>>
                (
                Expression.Equal(
                    Expression.Property(
                        itemParameter,
                        this.IDFieldName /*"_id"*/
                        ),
                    Expression.Constant(_id)
                    ),
                new[] { itemParameter }
                );

            var query = Builders<T>.Filter.Where(whereExpression);
            var foundItem = (T)collection.Find(query);
            return foundItem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FieldName"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public T GetByFieldValue<T>(string FieldName, object Value)
        {
            IMongoCollection<T> collection = Db.GetCollection<T>(CollectionName);

            List<Filter> filter = new List<Filter>()
            {
                new Filter { PropertyName = FieldName , Operation = Op.Equals, Value = Value }
            };

            var exp = ExpressionBuilder.GetExpression<T>(filter);
            var query = Builders<T>.Filter.Where(exp);

            var foundItem = (T)collection.Find(query);
            return foundItem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FieldName"></param>
        /// <returns></returns>
        public T GetLast<T>(string FieldName)
        {
            //TODO bunun icinde asagidaki gibi bir cagriyla cozebilir miyiz test edelim
            //return GetLast<T>(null, FieldName);

            IMongoCollection<T> collection = Db.GetCollection<T>(CollectionName);

            var filter = new BsonDocument();
            var sortBy = Builders<T>.Sort.Descending(FieldName);
            var foundItem = collection.Find(filter).Sort(sortBy).Limit(1).FirstOrDefault();

            return foundItem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Filter"></param>
        /// <param name="FieldName"></param>
        /// <returns></returns>
        public T GetLast<T>(List<Filter> Filter, string FieldName)
        {
            return GetLast<T>(Filter, FieldName, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Filter"></param>
        /// <param name="FieldName"></param>
        /// <param name="UseAndLogic"></param>
        /// <returns></returns>
        public T GetLast<T>(List<Filter> Filter, string FieldName, bool UseAndLogic = true)
        {
            IMongoCollection<T> collection = Db.GetCollection<T>(CollectionName);

            var exp = ExpressionBuilder.GetExpression<T>(Filter, UseAndLogic);
            var query = Builders<T>.Filter.Where(exp);
            var sortBy = Builders<T>.Sort.Descending(FieldName);

            var foundItem = collection.Find(query).Sort(sortBy).Limit(1).FirstOrDefault();
            return foundItem;
        }

        #endregion


        //DELETE DATA METHODS **************************************************************
        #region DELETE

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_id"></param>
        /// <returns></returns>
        public bool Delete<T>(ObjectId _id)
        {
            try
            {
                IMongoCollection<T> collection = Db.GetCollection<T>(CollectionName);

                List<Filter> filter = new List<Filter>()
                {
                    new Filter { PropertyName = /*"_id"*/ this.IDFieldName, Operation = Op.Equals, Value = _id }
                };

                var exp = ExpressionBuilder.GetExpression<T>(filter);
                var query = Builders<T>.Filter.Where(exp);

                DeleteResult result = collection.DeleteOne(query);

                return true;
            }
            catch (Exception ex)
            {
                //log exception code 
                return false;

                //throw ex;
            }
        }

        #endregion

    }

    /// <summary>
    /// Used to store parameter values to connect and operate on MongoDB
    /// 
    /// This class also contains KeyName static properties to access values of the settings file
    /// </summary>
    public class MongoDBConfig
    {
        /// <summary>
        /// Key name for JSON Settings file
        /// </summary>
        public static string KeyNameConnectionString = "MongoDBConfig:ConnectionString";

        /// <summary>
        /// Key name for JSON Settings file
        /// </summary>
        public static string KeyNameDatabaseName = "MongoDBConfig:DatabaseName";

        /// <summary>
        /// This parameter must contain MongoDB connection string
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// This is the database name to work with
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Default construtor with two required parameteters
        /// </summary>
        /// <param name="ConnectionString">Check class definition for details</param>
        /// <param name="DatabaseName">Check class definition for details</param>
        public MongoDBConfig(string ConnectionString, string DatabaseName)
        {
            this.ConnectionString = ConnectionString;
            this.DatabaseName = DatabaseName;
        }
    }
}
