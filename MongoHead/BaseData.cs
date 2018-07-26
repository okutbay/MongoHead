using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoHead.Interfaces;

namespace MongoHead
{
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
        readonly IConfiguration _configuration;
        public readonly MongoDBConfig MongoDBConfig;

        string CollectionName { get; set; }

        MongoDBHelper Helper { get; set; }

        /// <summary>
        /// "IDFieldName" constant is used to access specific "_id" field property name of the base entity to access it in run-time for insert, update or delete purposes
        /// </summary>
        public string IDFieldName { get { return MongoDBHelper.BsonDocumentIDFieldName; } }

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

        public BaseData(IConfiguration Configuration)
        {
            this._configuration = Configuration;

            //These settings are checked on the helper class
            MongoDBConfig = new MongoDBConfig(
                _configuration[MongoDBConfig.KeyNameConnectionString],
                _configuration[MongoDBConfig.KeyNameDatabaseName]
                );

            this.CollectionName = typeof(T).Name;

            //Set Helper Instance
            this.Helper = new MongoDBHelper(this.MongoDBConfig, typeof(T));
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
        /// Inserts or updates
        /// </summary>
        /// <param name="ObjectToSave"></param>
        /// <returns></returns>
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
                dateCreatedProperty.SetValue(ObjectToSave, currentTime);
                dateModifiedProperty.SetValue(ObjectToSave, currentTime);

                newId = Helper.Insert(ObjectToSave);
            }
            else //this operation is a update
            {
                //set only modify date
                dateModifiedProperty.SetValue(ObjectToSave, currentTime);

                newId = Helper.Replace(ObjectToSave, currentId);
            }

            //pass new id value to the incoming object
            idProperty.SetValue(ObjectToSave, newId);

            //also return new id
            return newId;
        }


    }
}
