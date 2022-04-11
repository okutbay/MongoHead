using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoHead;

public class MongoDBHelper<TT>
{
    #region CONSTS

    /// <summary>
    /// Constant for Id name of our collections. Used to access specific "_id" field property name of the BSON document to access it in run-time for insert, update or delete purposes
    /// </summary>
    public const string BsonDocumentIDFieldName = "_id";

    /// <summary>
    /// MongoHead specific Create Date field Name
    /// </summary>
    public const string DateUtcCreatedFieldName = "_DateUtcCreated";

    /// <summary>
    /// MongoHead specific Modified Date field Name
    /// </summary>
    public const string DateUtcModifiedFieldName = "_DateUtcModified";

    /// <summary>
    /// MongoHead specific is active status field Name
    /// </summary>
    public const string IsActiveFieldName = "_IsActive";

    #endregion

    /// <summary>
    /// Will have Configuration instance from DI after construction
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Mongo DB Config values from appsettings.josn are stored in this object
    /// </summary>
    private readonly Config config;

    /// <summary>
    /// MongoDB Database instance
    /// </summary>
    public IMongoDatabase Db { get; set; }

    /// <summary>
    /// Collection instance
    /// </summary>
    public IMongoCollection<TT> Collection { get; set; }

    /// <summary>
    /// Name of the collection. Taken from type TT
    /// </summary>
    public string CollectionName { get; set; }

    #region CONSTRUCTORS

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="IConfiguration"></param>
    /// <param name="EntityType"></param>
    /// 
    [Obsolete("This constructor is obsolete. Please use others.")]
    public MongoDBHelper(IConfiguration configuration, Type EntityType)
    {
        _configuration = configuration;

        config = new Config(_configuration);

        this.CollectionName = EntityType.Name;
        this.Db = this.GetDBInstance();
        this.Collection = this.GetCollectionInstance();
    }

    /// <summary>
    /// Construtor with IConfiguration
    /// </summary>
    /// <param name="IConfiguration"></param>
    public MongoDBHelper(IConfiguration configuration)
    {
        _configuration = configuration;

        config = new Config(_configuration);

        if (string.IsNullOrEmpty(this.config.ConnectionString))
        {
            throw new Exception("MongoHead.MongoDBHelper config error: invalid or undefined connection string setting");
        }

        if (string.IsNullOrEmpty(this.config.DatabaseName))
        {
            throw new Exception("MongoHead.MongoDBHelper config error: DBName is not set. Please check your DatabaseName setting");
        }

        Type EntityType = typeof(TT);

        this.CollectionName = EntityType.Name;
        this.Db = this.GetDBInstance();
        this.Collection = this.GetCollectionInstance();
    }

    /// <summary>
    /// Construtor with ConnectionString and DbName
    /// </summary>
    /// <param name="ConnectionString"></param>
    /// <param name="DbName"></param>
    public MongoDBHelper(string ConnectionString, string DbName)
    {
        if (string.IsNullOrEmpty(ConnectionString))
        {
            throw new Exception("MongoHead.MongoDBHelper config error: ConnectionString is not set.");
        }

        if (string.IsNullOrEmpty(DbName))
        {
            throw new Exception("MongoHead.MongoDBHelper config error: DbName is not set.");
        }

        config = new Config(ConnectionString, DbName);

        Type EntityType = typeof(TT);

        this.CollectionName = EntityType.Name;
        this.Db = this.GetDBInstance();
        this.Collection = this.GetCollectionInstance();
    } 

    #endregion

    #region SAVE

    /// <summary>
    /// Save Object to DB
    /// </summary>
    /// <param name="ObjectToSave">Object To Save</param>
    /// <returns>New ObjectId for saved object</returns>
    public ObjectId Insert(object ObjectToSave)
    {
        BsonDocument document = removeUnwantedPrefix_t(ObjectToSave);
        ObjectId newId = this.Insert(document);

        return newId;
    }

    /// <summary>
    /// Save BsonDocument to DB
    /// </summary>
    /// <param name="BsonDocumentToSave">BsonDocument To Save</param>
    /// <returns>New ObjectId for saved BsonDocument</returns>
    public ObjectId Insert(BsonDocument BsonDocumentToSave)
    {
        IMongoCollection<BsonDocument> collection = Db.GetCollection<BsonDocument>(CollectionName);
        collection.InsertOne(BsonDocumentToSave);
        string id = BsonDocumentToSave[MongoDBHelper<TT>.BsonDocumentIDFieldName].ToString();
        ObjectId newId = new ObjectId(id);

        return newId;
    }

    /// <summary>
    /// Update document with ObjectId with given object
    /// </summary>
    /// <param name="ObjectToReplace">Object To Replace</param>
    /// <returns>ObjectId for updated document</returns>
    public ObjectId Replace(object ObjectToReplace, ObjectId Id)
    {
        BsonDocument document = removeUnwantedPrefix_t(ObjectToReplace);
        ObjectId id = this.Replace(document, Id);

        return id;
    }

    /// <summary>
    /// Update document with ObjectId with given BsonDocument
    /// </summary>
    /// <param name="BsonDocumentToReplace">BsonDocument To Replace</param>
    /// <returns>ObjectId for updated document</returns>
    public ObjectId Replace(BsonDocument BsonDocumentToReplace, ObjectId Id)
    {
        IMongoCollection<BsonDocument> collection = Db.GetCollection<BsonDocument>(CollectionName);
        var filter = Builders<BsonDocument>.Filter.Eq(MongoDBHelper<TT>.BsonDocumentIDFieldName, Id);
        collection.ReplaceOne(filter, BsonDocumentToReplace);

        return Id;
    }

    #endregion

    #region GET LIST

    /// <summary>
    /// Get list of given type
    /// </summary>
    /// <typeparam name="T">Generic Type</typeparam>
    /// <returns>List<T></returns>
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
    /// Returns documents matches ObjectId value in the field with specified KeyFieldName. This field may be default _id field or another field with ObjectId
    /// </summary>
    /// <typeparam name="T">Generic Type</typeparam>
    /// <param name="Key"></param>
    /// <param name="KeyFieldName"></param>
    /// <returns>List<T></returns>
    public List<T> GetList<T>(ObjectId Key, string KeyFieldName)
    {
        IMongoCollection<T> collection = Db.GetCollection<T>(CollectionName);
        var filter = Builders<T>.Filter.Eq(KeyFieldName, Key);
        var foundItems = collection.Find(filter).ToList();

        return foundItems;
    }

    /// <summary>
    /// Get list of collection that matches to filter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Filter"></param>
    /// <returns>List<T></returns>
    public List<T> GetList<T>(List<Filter> Filter)
    {
        List<T> foundItems = GetList<T>(Filter, true);

        return foundItems;
    }

    /// <summary>
    /// Get list of collection that matches to filter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Filter"></param>
    /// <param name="UseAndLogic"></param>
    /// <returns>List<T></returns>
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

    #region GET

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Filter"></param>
    /// <returns></returns>
    public T Get<T>(List<Filter> Filter)
    {
        T foundItem = Get<T>(Filter, true);
        return foundItem;
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
        var filter = Builders<T>.Filter.Eq(MongoDBHelper<T>.BsonDocumentIDFieldName /*"_id"*/, _id);
        var foundItem = (T)collection.Find(filter).FirstOrDefault();
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

        var foundItem = (T)collection.Find(query).FirstOrDefault();
        return foundItem;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="SortFieldName"></param>
    /// <returns></returns>
    public T GetLast<T>(string SortFieldName)
    {
        //TODO bunun icinde asagidaki gibi bir cagriyla cozebilir miyiz test edelim
        //return GetLast<T>(null, FieldName);

        IMongoCollection<T> collection = Db.GetCollection<T>(CollectionName);

        var filter = new BsonDocument();
        var sortBy = Builders<T>.Sort.Descending(SortFieldName);
        var foundItem = collection.Find(filter).Sort(sortBy).Limit(1).FirstOrDefault();

        return foundItem;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Filter"></param>
    /// <param name="SortFieldName"></param>
    /// <returns></returns>
    public T GetLast<T>(List<Filter> Filter, string SortFieldName)
    {
        var foundItem = GetLast<T>(Filter, SortFieldName, true);
        return foundItem;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Filter"></param>
    /// <param name="SortFieldName"></param>
    /// <param name="UseAndLogic"></param>
    /// <returns></returns>
    public T GetLast<T>(List<Filter> Filter, string SortFieldName, bool UseAndLogic = true)
    {
        IMongoCollection<T> collection = Db.GetCollection<T>(CollectionName);

        var exp = ExpressionBuilder.GetExpression<T>(Filter, UseAndLogic);
        var query = Builders<T>.Filter.Where(exp);
        var sortBy = Builders<T>.Sort.Descending(SortFieldName);

        var foundItem = collection.Find(query).Sort(sortBy).Limit(1).FirstOrDefault();
        return foundItem;
    }

    #endregion

    #region DELETE

    /// <summary>
    /// Delete a document by Id from the collection T
    /// </summary>
    /// <typeparam name="T">Type for collection</typeparam>
    /// <param name="_id">Object Id value to delete from collection</param>
    /// <returns>Boolean</returns>
    public bool Delete<T>(ObjectId _id)
    {
        try
        {
            IMongoCollection<T> collection = Db.GetCollection<T>(CollectionName);

            List<Filter> filter = new List<Filter>()
                {
                    new Filter { 
                        PropertyName = BsonDocumentIDFieldName/* _id */, 
                        Operation = Op.Equals, 
                        Value = _id }
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

    /// <summary>
    /// Deletes all documents that matches the value in the named field.
    /// </summary>
    /// <typeparam name="T">Related Entity (Collection) type</typeparam>
    /// <param name="FieldName">Field Name to match the Value</param>
    /// <param name="Value">Value to match in field named with FieldName</param>
    /// <param name="DeletedCount">Number of deleted documents</param>
    /// <returns>Boolean</returns>
    public bool DeleteByFieldValue<T>(string FieldName, object Value, out long DeletedCount)
    {
        try
        {
            IMongoCollection<T> collection = Db.GetCollection<T>(CollectionName);

            List<Filter> filter = new List<Filter>()
                {
                    new Filter { 
                        PropertyName = FieldName, 
                        Operation = Op.Equals, 
                        Value = Value }
                };

            var exp = ExpressionBuilder.GetExpression<T>(filter);
            var query = Builders<T>.Filter.Where(exp);

            DeleteResult result = collection.DeleteMany(query);
            DeletedCount = result.DeletedCount;

            return result.IsAcknowledged;
        }
        catch (Exception ex)
        {
            //log exception code 
            DeletedCount = 0;
            return false;
            //throw ex;
        }
    }

    #endregion

    #region GENERAL DATABASE

    /// <summary>
    /// Key to filter documents by name
    /// </summary>
    private const string collectionNameKey = "name";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="CollectionName"></param>
    /// <returns></returns>
    public bool CollectionExists(string CollectionName)
    {
        var filter = new BsonDocument(collectionNameKey, CollectionName);
        var options = new ListCollectionNamesOptions { Filter = filter };

        return Db.ListCollectionNames(options).Any();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="CollectionName"></param>
    /// <returns></returns>
    public async Task<bool> CollectionExistsAsync(string CollectionName)
    {
        var filter = new BsonDocument(collectionNameKey, CollectionName);
        var collections = await Db.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });

        return await collections.AnyAsync();
    }

    #endregion

    #region SOME HELPERS

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ObjectToSave"></param>
    /// <returns></returns>
    private static BsonDocument removeUnwantedPrefix_t(object ObjectToSave)
    {
        /// <summary>
        /// MongoHead specific prefix const (Conversion to BsonDocument adds _t as type of the object to the name. 
        /// We don't want this "_t" prefix and removing from documents.)
        /// </summary>
        const string unwantedPrefix_t = "_t";

        BsonDocument document = ObjectToSave.ToBsonDocument();
        document.Remove(unwantedPrefix_t);

        return document;
    }

    /// <summary>
    /// Returns an instance of Mongo Database with database name defined in the config object. 
    /// </summary>
    /// <returns>IMongoDatabase</returns>
    private IMongoDatabase GetDBInstance()
    {
        if (this.config == null)
        {
            throw new Exception("No config instance to create DB Instance.");
        }

        MongoClient client = new MongoClient(this.config.ConnectionString);
        IMongoDatabase _db = client.GetDatabase(this.config.DatabaseName);

        return _db;
    }

    /// <summary>
    /// Use type name to return a collection instance
    /// </summary>
    /// <returns>IMongoCollection<TT> collection instance</returns>
    private IMongoCollection<TT> GetCollectionInstance()
    {
        var collectionName = typeof(TT).Name;
        var collection = Db.GetCollection<TT>(collectionName);

        return collection;
    }

    /// <summary>
    /// Runs command.
    /// </summary>
    /// <param name="CommandString">Command</param>
    /// <returns></returns>
    public async Task RunCommandAsync(string CommandString)
    {
        var command = BsonDocument.Parse(CommandString);
        await this.Db.RunCommandAsync<BsonDocument>(command);
    } 

    #endregion

}
