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
    public class BaseData<T> : IBaseData<T>
    {
        readonly IConfiguration _configuration;
        readonly MongoDBConfig config;


        string CollectionName { get; set; }
        

        public BaseData(IConfiguration Configuration)
        {
            this._configuration = Configuration;
            //TODO bu keyler yok ise ne oluyor??? deneyelim

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

        public T GetById(string Id)
        {
            ObjectId id = new ObjectId(Id);
            T result = GetById(id);
            return result;
        }

        public T GetById(ObjectId Id)
        {
            //Get Helper Instance
            MongoDBHelper helper = new MongoDBHelper(this.config);

            T foundItem = helper.Get<T>(CollectionName, Id);
            return foundItem;
        }

        #endregion


        public ObjectId Save(T ObjectToSave)
        {
            //Get Helper Instance
            MongoDBHelper helper = new MongoDBHelper(this.config);

            //TODO auto add date fields???
            //check id value first.

            PropertyInfo idProperty = typeof(T).GetProperty(helper.IDFieldName);
            ObjectId currentId = (ObjectId)idProperty.GetValue(ObjectToSave);
            ObjectId emptyId = new ObjectId("000000000000000000000000");

            PropertyInfo dateCreatedProperty = typeof(T).GetProperty(helper.DateCreatedFieldName);
            PropertyInfo dateModifiedProperty = typeof(T).GetProperty(helper.DateModifiedFieldName);

            DateTime currentTime = DateTime.UtcNow;

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

            PropertyInfo idProperty2 = typeof(T).GetProperty(helper.IDFieldName);
            idProperty2.SetValue(ObjectToSave, newId);

            return newId;
        }


    }
}
