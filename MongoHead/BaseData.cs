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
        readonly MongoDBConfig config;

        string CollectionName { get; set; }

        public BaseData(IConfiguration Configuration)
        {
            this._configuration = Configuration;

            //These settings are checked on the helper class
            config = new MongoDBConfig(
                _configuration["Settings:MongoDB:ConnectionString"],
                _configuration["Settings:MongoDB:DefaultDatabaseName"]
                );

            this.CollectionName = typeof(T).Name;
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
            //Get Helper Instance
            MongoDBHelper helper = new MongoDBHelper(this.config);

            bool result = helper.Delete<T>(CollectionName, Id);
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
            //Get Helper Instance
            MongoDBHelper helper = new MongoDBHelper(this.config);

            List<T> list = helper.GetList<T>(CollectionName);
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
            //Get Helper Instance
            MongoDBHelper helper = new MongoDBHelper(this.config);

            List<T> foundItems = helper.GetList<T>(CollectionName, filter, "", UseAndLogic);
            return foundItems;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="UseAndLogic"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetKeyValueList(List<Filter> filter, bool UseAndLogic = true)
        {
            //Get Helper Instance
            MongoDBHelper helper = new MongoDBHelper(this.config);

            string keyFieldName = helper.IDFieldName;
            string valueFieldName = $"{CollectionName}Name"; //string.Format("{0}Name", collectionName);

            Dictionary<string, string> dict;

            dict = GetKeyValueList(keyFieldName, valueFieldName, filter, UseAndLogic);

            return dict;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="KeyFieldName"></param>
        /// <param name="ValueFieldName"></param>
        /// <param name="filter"></param>
        /// <param name="UseAndLogic"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetKeyValueList(string KeyFieldName, string ValueFieldName, List<Filter> filter, bool UseAndLogic = true)
        {
            //Get Helper Instance
            MongoDBHelper helper = new MongoDBHelper(this.config);

            List<T> foundItems = helper.GetList<T>(CollectionName, filter, "", UseAndLogic);

            PropertyInfo idProperty = typeof(T).GetProperty(KeyFieldName);
            PropertyInfo nameProperty = typeof(T).GetProperty(ValueFieldName);

            if (idProperty == null)
            {
                throw new Exception(string.Format("Unable to reflect key property. please check entity \"{0}\" contains \"{1}\" property.", CollectionName, KeyFieldName));
            }

            if (nameProperty == null)
            {
                throw new Exception(string.Format("Unable to reflect name property. please check entity \"{0}\" contains \"{1}\" property.", CollectionName, ValueFieldName));
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
            //Get Helper Instance
            MongoDBHelper helper = new MongoDBHelper(this.config);

            T foundItem = helper.Get<T>(CollectionName, Id);
            return foundItem;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ObjectToSave"></param>
        /// <returns></returns>
        public ObjectId Save(T ObjectToSave)
        {
            //Get Helper Instance
            MongoDBHelper helper = new MongoDBHelper(this.config);

            PropertyInfo idProperty = typeof(T).GetProperty(helper.IDFieldName);
            PropertyInfo dateCreatedProperty = typeof(T).GetProperty(helper.DateUtcCreatedFieldName);
            PropertyInfo dateModifiedProperty = typeof(T).GetProperty(helper.DateUtcModifiedFieldName);

            ObjectId currentId = (ObjectId)idProperty.GetValue(ObjectToSave);//Get incoming object's Id value
            ObjectId emptyId = new ObjectId();//Create an Empty Object Id with default value of {000000000000000000000000}

            DateTime currentTime = DateTime.UtcNow;

            //check id value first.
            if (currentId == emptyId) //this operation is a new insert
            {
                //set create and modify dates
                dateCreatedProperty.SetValue(ObjectToSave, currentTime);
                dateModifiedProperty.SetValue(ObjectToSave, currentTime);
            }
            else //this operation is a update
            {
                //set only modify date
                dateModifiedProperty.SetValue(ObjectToSave, currentTime);
            }

            ObjectId newId = helper.Save(this.CollectionName, ObjectToSave);

            //pass new id value to the incoming object
            idProperty.SetValue(ObjectToSave, newId);

            //also return new id
            return newId;
        }


    }
}
