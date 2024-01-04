using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoHead.Interfaces;

namespace MongoHead;

/// <summary>
/// This class wraps helper methods and provides base data methods for your data class
/// Data class inheriting this class must have "internal" accessor to prevent direct access from presantation apps/layers
/// </summary>
/// <typeparam name="T">Generic Type which presents your Entity Object</typeparam>
/// <example>
/// Sample data class implementation with configuration injection
/// <code>
/// internal class SomeData : MongoHead.BaseData<SomeEntity>
/// {
///   readonly IConfiguration _configuration;
///
///   public SomeData(IConfiguration Configuration) : base(Configuration)
///   {
///     this._configuration = Configuration;
///   }
/// }
/// </code>
/// </example>
public class BaseData<T> : IBaseData<T>
{
    private readonly IConfiguration _configuration;

    public string CollectionName { get; set; }

    public MongoDBHelper<T> Helper { get; set; }

    /// <summary>
    /// "IDFieldName" constant is used to access specific "_id" field property name of the base entity to access it in run-time for insert, update or delete purposes
    /// </summary>
    public string IDFieldName { get { return MongoDBHelper<T>.BsonDocumentIDFieldName; } }

    /// <summary>
    /// "DateCreatedFieldName" constant is used to access specific "_DateCreated" field property name of the base entity to access it in run-time for insert, update or delete purposes
    /// </summary>
    public string DateUtcCreatedFieldName { get { return MongoDBHelper<T>.DateUtcCreatedFieldName; } }

    /// <summary>
    /// "DateModifiedFieldName" constant is used to access specific "_DateModified" field property name of the base entity to access it in run-time for insert, update or delete purposes
    /// </summary>
    public string DateUtcModifiedFieldName { get { return MongoDBHelper<T>.DateUtcModifiedFieldName; } }

    /// <summary>
    /// "IsActiveFieldName" constant is used to access specific "_IsActive" field property name of the base entity to access it in run-time for insert, update or delete purposes
    /// </summary>
    public string IsActiveFieldName { get { return MongoDBHelper<T>.IsActiveFieldName; } }

    public BaseData(IConfiguration Configuration)
    {
        this._configuration = Configuration;

        //Set Helper Instance
        this.Helper = new MongoDBHelper<T>(_configuration);
        this.CollectionName = Helper.CollectionName;
        //this.CollectionName = typeof(T).Name;
    }

    /// <summary>
    /// Use specific connection to access custom database other than defined in configuration
    /// </summary>
    /// <param name="ConnectionString">Connection string</param>
    /// <param name="DbName">DB To use</param>
    public BaseData(string ConnectionString, string DbName)
    {
        //Set Helper Instance
        this.Helper = new MongoDBHelper<T>(ConnectionString, DbName);
        this.CollectionName = Helper.CollectionName;
        //this.CollectionName = typeof(T).Name;
    }

    #region Delete

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public bool Delete(string Id)
    {
        ObjectId id = new ObjectId(Id);
        bool result = this.Delete(id);
        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public bool Delete(ObjectId Id)
    {
        bool result = this.Helper.Delete<T>(Id);
        return result;
    }

    #endregion


    #region Get List

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public List<T> GetList()
    {
        List<T> list = Helper.GetList<T>();
        return list;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="UseAndLogic"></param>
    /// <returns></returns>
    public List<T> GetList(List<Filter> filter, bool UseAndLogic = true)
    {
        List<T> foundItems = Helper.GetList<T>(filter, UseAndLogic);
        return foundItems;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Filter"></param>
    /// <param name="UseAndLogic"></param>
    /// <returns></returns>
    public Dictionary<string, string> GetKeyValueList(List<Filter> Filter, bool UseAndLogic = true)
    {
        string keyFieldName = this.IDFieldName;
        string valueFieldName = $"{CollectionName}Value";

        Dictionary<string, string> dict;

        dict = GetKeyValueList(keyFieldName, valueFieldName, Filter, UseAndLogic);

        return dict;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="KeyFieldName"></param>
    /// <param name="ValueFieldName"></param>
    /// <param name="Filter"></param>
    /// <param name="UseAndLogic"></param>
    /// <returns></returns>
    public Dictionary<string, string> GetKeyValueList(string KeyFieldName, string ValueFieldName, List<Filter> filter, bool UseAndLogic = true)
    {
        List<T> foundItems = Helper.GetList<T>(filter, UseAndLogic);

        PropertyInfo idProperty = typeof(T).GetProperty(KeyFieldName);
        PropertyInfo nameProperty = typeof(T).GetProperty(ValueFieldName);

        if (idProperty == null)
        {
            var message = $"Unable to reflect key property. Please check entity \"{CollectionName}\" contains \"{KeyFieldName}\" property.";
            throw new Exception(message);
        }

        if (nameProperty == null)
        {
            var message = $"Unable to reflect key property. Please check entity \"{CollectionName}\" contains \"{ValueFieldName}\" property.";
            throw new Exception(message);
        }

        Dictionary<string, string> dict = new Dictionary<string, string>();

        foreach (T item in foundItems)
        {
            string key = idProperty.GetValue(item).ToString();
            string value = nameProperty.GetValue(item).ToString();
            dict.Add(key, value);
        }

        return dict;
    }

    #endregion


    #region Get By Id

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public T GetById(string Id)
    {
        ObjectId id = new ObjectId(Id);
        T result = GetById(id);
        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public T GetById(ObjectId Id)
    {
        T foundItem = Helper.GetByObjectId<T>(Id);
        return foundItem;
    }

    #endregion

    /// <summary>
    /// Inserts or updates.
    /// If the object has Id field populated this means an update 
    /// and we are expecting a full object which is previously retrived from DB.
    /// For that reason on updates we are only modifying DateUtcModified property.
    /// </summary>
    /// <param name="ObjectToSave"></param>
    /// <returns>Id value for the object</returns>
    public ObjectId Save(T ObjectToSave)
    {
        PropertyInfo idProperty = typeof(T).GetProperty(this.IDFieldName);
        PropertyInfo dateCreatedProperty = typeof(T).GetProperty(this.DateUtcCreatedFieldName);
        PropertyInfo dateModifiedProperty = typeof(T).GetProperty(this.DateUtcModifiedFieldName);

        ObjectId currentId = (ObjectId)idProperty.GetValue(ObjectToSave);//Get incoming object's Id value
        ObjectId emptyId = new ObjectId();//Create an Empty Object Id with default value of {000000000000000000000000}
        ObjectId newId = new ObjectId();

        DateTime currentTime = DateTime.UtcNow;

        //check id value first.
        if (currentId == emptyId) //this operation is a new insert
        {
            //set create and modify dates
            dateCreatedProperty?.SetValue(ObjectToSave, currentTime);
            dateModifiedProperty?.SetValue(ObjectToSave, currentTime);

            newId = Helper.Insert(ObjectToSave);
        }
        else //this operation is a update
        {
            //set only modify date
            dateModifiedProperty?.SetValue(ObjectToSave, currentTime);

            newId = Helper.Replace(ObjectToSave, currentId);
        }

        //pass new id value to the incoming object
        idProperty?.SetValue(ObjectToSave, newId);

        //also return new id
        return newId;
    }

    public async Task<List<T>> GetListAsync()
    {
        var collection = Helper.Collection;
        var results = await collection.FindAsync(_ => true);
        return results.ToList();
    }

    public Task<List<T>> GetListAsync(List<Filter> Filter, bool UseAndLogic = true)
    {
        //var filter = Builders<T>.Filter.Eq("_id", "");
        //FilterDefinition<T> nameFilter = Builders<T>.Filter.Eq("_id", "");
        //FilterDefinition inStockFilter = Builders<T>.Filter.Eq(x => x.InStock, true);
        //FilterDefinition combineFilters = Builders<T>.Filter.And(nameFilter, inStockFilter);

        throw new NotImplementedException();
    }

    public Task SaveAsync(T ObjectToSave)
    {
        var collection = Helper.Collection;
        return collection.InsertOneAsync(ObjectToSave);

        //return collection.ReplaceOneAsync(ObjectToSave, new ReplaceOptions { IsUpsert = true });
    }

    /// <summary>
    /// Creates a descending index for base date fields to expire after duration of time
    /// </summary>
    /// <param name="IndexName">Name of the Index</param>
    /// <param name="Duration">Duration to keep documents in the collection</param>
    /// <param name="ModifiedDate">Use Modified Date on collection or not. If false method created index for Create date</param>
    /// <returns></returns>
    public async Task CreateIndexExpireAfterDuration(string IndexName, TimeSpan Duration, bool ModifiedDate=false)
    {
        var fieldName = string.Empty;
        string duration = Duration.TotalSeconds.ToString();

        if (ModifiedDate)
        {
            fieldName = DateUtcModifiedFieldName;
        }
        else
        {
            fieldName = DateUtcCreatedFieldName;
        }

        var commandString = "{ createIndexes: '" + CollectionName + "', indexes: [ { key: { " + fieldName + ": -1 }, name: '" + IndexName + "', unique: false, expireAfterSeconds: "+ duration + ", sparse: true, background: true } ] }";
        await Helper.RunCommandAsync(commandString);
    }
}
