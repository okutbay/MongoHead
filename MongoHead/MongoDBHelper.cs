using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoHead
{
    public class MongoDBHelper
    {
        readonly MongoDBConfig _Config;

        /// <summary>
        /// "IDFieldName" constant is used to access specific "_id" field property name of the base entity to access it in run-time for insert, update or delete purposes
        /// </summary>
        public string IDFieldName { get { return "_id"; } }

        /// <summary>
        /// "DateCreatedFieldName" constant is used to access specific "_DateCreated" field property name of the base entity to access it in run-time for insert, update or delete purposes
        /// </summary>
        public string DateCreatedFieldName { get { return "_DateCreated"; } }

        /// <summary>
        /// "DateModifiedFieldName" constant is used to access specific "_DateModified" field property name of the base entity to access it in run-time for insert, update or delete purposes
        /// </summary>
        public string DateModifiedFieldName { get { return "_DateModified"; } }

        /// <summary>
        /// "IsActiveFieldName" constant is used to access specific "_IsActive" field property name of the base entity to access it in run-time for insert, update or delete purposes
        /// </summary>
        public string IsActiveFieldName { get { return "_IsActive"; } }

        public MongoDBHelper(MongoDBConfig Config)
        {
            _Config = Config;
        }

        /// <summary>
        /// Returns an instance of Mongo Database for Prefferred or default database name. 
        /// </summary>
        /// <param name="PrefferedDBName">This parameter is used to swith to another database then default database. 
        /// With this parameter you do not have use default database to store your collections. 
        /// Leave empty to use default database name defined in the application settings.</param>
        /// <returns></returns>
        private IMongoDatabase GetDBInstance(string PrefferedDBName = "")
        {
            string connectionString = string.Empty;
            string dbName = string.Empty;

            connectionString = this._Config.ConnectionString;

            //if no specific database name is given operations will be carried on default database 
            if (!string.IsNullOrEmpty(PrefferedDBName))
            {
                dbName = PrefferedDBName;
            }
            else
            {
                dbName = this._Config.DefaultDatabaseName;
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("MongoHead.MongoDBHelper config error: invalid or undefined connection string setting");
            }

            if (string.IsNullOrEmpty(dbName))
            {
                throw new Exception("MongoHead.MongoDBHelper config error: DBName is not set. Please check your DefaultDatabaseName setting or PreferredDBName");
            }

            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase _db = client.GetDatabase(dbName);//we are not storing the dbname to instance. Every helper method can work on different databases

            return _db;
        }


        //SAVE DATA METHODS **************************************************************
        #region SAVE

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CollectionName"></param>
        /// <param name="ObjectToSave"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public ObjectId Save(string CollectionName, object ObjectToSave, string dbName = "")
        {
            IMongoDatabase db = this.GetDBInstance(dbName);
            IMongoCollection<BsonDocument> collection = db.GetCollection<BsonDocument>(CollectionName);

            BsonDocument document = ObjectToSave.ToBsonDocument(); //conversion to BsonDocument adds _t as type of the object to the 
            document.Remove("_t"); //we dont want this just remove

            ObjectId newId = this.Save(CollectionName, document, dbName);
            return newId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CollectionName"></param>
        /// <param name="ObjectToSave"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public ObjectId Save(string CollectionName, BsonDocument ObjectToSave, string dbName = "")
        {
            IMongoDatabase db = this.GetDBInstance(dbName);
            IMongoCollection<BsonDocument> collection = db.GetCollection<BsonDocument>(CollectionName);

            //TODO save islemlerinde datecreated ve date modified biz set edelim.
            DateTime DateCreated = ObjectToSave[DateCreatedFieldName].ToUniversalTime();
            DateTime DateModified = ObjectToSave[DateModifiedFieldName].ToUniversalTime();

            collection.InsertOne(ObjectToSave);

            string id = ObjectToSave[IDFieldName].ToString();

            ObjectId newId = new ObjectId(id);
            return newId;
        }

        #endregion


        // GET DATA METHODS **************************************************************
        #region GET

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CollectionName"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public List<T> GetList<T>(string CollectionName, string dbName = "")
        {
            IMongoDatabase db = this.GetDBInstance(dbName);
            IMongoCollection<T> collection = db.GetCollection<T>(CollectionName);

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
        /// <param name="CollectionName"></param>
        /// <param name="Filter"></param>
        /// <returns></returns>
        public List<T> GetList<T>(string CollectionName, List<Filter> Filter)
        {
            return GetList<T>(CollectionName, Filter, string.Empty, true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CollectionName"></param>
        /// <param name="Filter"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public List<T> GetList<T>(string CollectionName, List<Filter> Filter, string dbName = "")
        {
            return GetList<T>(CollectionName, Filter, dbName, true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CollectionName"></param>
        /// <param name="Filter"></param>
        /// <param name="UseAndLogic"></param>
        /// <returns></returns>
        public List<T> GetList<T>(string CollectionName, List<Filter> Filter, bool UseAndLogic = true)
        {
            return GetList<T>(CollectionName, Filter, string.Empty, UseAndLogic);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CollectionName"></param>
        /// <param name="Filter"></param>
        /// <param name="dbName"></param>
        /// <param name="UseAndLogic"></param>
        /// <returns></returns>
        public List<T> GetList<T>(string CollectionName, List<Filter> Filter, string dbName = "", bool UseAndLogic = true)
        {
            IMongoDatabase db = this.GetDBInstance(dbName);
            IMongoCollection<T> collection = db.GetCollection<T>(CollectionName);

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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CollectionName"></param>
        /// <param name="_id"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public T Get<T>(string CollectionName, ObjectId _id, string dbName = "")
        {
            IMongoDatabase db = this.GetDBInstance(dbName);
            IMongoCollection<T> collection = db.GetCollection<T>(CollectionName);

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
        /// <param name="CollectionName"></param>
        /// <param name="FieldName"></param>
        /// <param name="Value"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public T Get<T>(string CollectionName, string FieldName, object Value, string dbName = "")
        {
            IMongoDatabase db = this.GetDBInstance(dbName);
            IMongoCollection<T> collection = db.GetCollection<T>(CollectionName);

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
        /// <param name="CollectionName"></param>
        /// <param name="Filter"></param>
        /// <returns></returns>
        public T Get<T>(string CollectionName, List<Filter> Filter)
        {
            return Get<T>(CollectionName, Filter, string.Empty, true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CollectionName"></param>
        /// <param name="Filter"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public T Get<T>(string CollectionName, List<Filter> Filter, string dbName = "")
        {
            return Get<T>(CollectionName, Filter, dbName, true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CollectionName"></param>
        /// <param name="Filter"></param>
        /// <param name="UseAndLogic"></param>
        /// <returns></returns>
        public T Get<T>(string CollectionName, List<Filter> Filter, bool UseAndLogic = true)
        {
            return Get<T>(CollectionName, Filter, string.Empty, UseAndLogic);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CollectionName"></param>
        /// <param name="Filter"></param>
        /// <param name="dbName"></param>
        /// <param name="UseAndLogic"></param>
        /// <returns></returns>
        public T Get<T>(string CollectionName, List<Filter> Filter, string dbName = "", bool UseAndLogic = true)
        {
            IMongoDatabase db = this.GetDBInstance(dbName);
            IMongoCollection<T> collection = db.GetCollection<T>(CollectionName);

            var exp = ExpressionBuilder.GetExpression<T>(Filter, UseAndLogic);
            var query = Builders<T>.Filter.Where(exp);

            var foundItem = (T)collection.Find(query);

            return foundItem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CollectionName"></param>
        /// <param name="FieldName"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public T GetLast<T>(string CollectionName, string FieldName, string dbName = "")
        {
            IMongoDatabase db = this.GetDBInstance(dbName);
            IMongoCollection<T> collection = db.GetCollection<T>(CollectionName);

            var filter = new BsonDocument();
            var sortBy = Builders<T>.Sort.Descending(FieldName);
            var foundItem = collection.Find(filter).Sort(sortBy).Limit(1).FirstOrDefault();

            return foundItem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CollectionName"></param>
        /// <param name="Filter"></param>
        /// <param name="FieldName"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public T GetLast<T>(string CollectionName, List<Filter> Filter, string FieldName, string dbName = "")
        {
            return GetLast<T>(CollectionName, Filter, FieldName, dbName, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CollectionName"></param>
        /// <param name="Filter"></param>
        /// <param name="FieldName"></param>
        /// <param name="dbName"></param>
        /// <param name="UseAndLogic"></param>
        /// <returns></returns>
        public T GetLast<T>(string CollectionName, List<Filter> Filter, string FieldName, string dbName, bool UseAndLogic = true)
        {
            IMongoDatabase db = this.GetDBInstance(dbName);
            IMongoCollection<T> collection = db.GetCollection<T>(CollectionName);

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
        /// <param name="CollectionName"></param>
        /// <param name="_id"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public bool Delete<T>(string CollectionName, ObjectId _id, string dbName = "")
        {
            try
            {
                IMongoDatabase db = this.GetDBInstance(dbName);
                IMongoCollection<T> collection = db.GetCollection<T>(CollectionName);

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
    /// 
    /// </summary>
    public class MongoDBConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DefaultDatabaseName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="DefaultDatabaseName"></param>
        public MongoDBConfig(string ConnectionString, string DefaultDatabaseName)
        {
            this.ConnectionString = ConnectionString;
            this.DefaultDatabaseName = DefaultDatabaseName;
        }
    }
}
