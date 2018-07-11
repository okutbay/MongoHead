using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using System.Reflection;

using Microsoft.Extensions.Configuration;

namespace MongoHead
{
    public class BaseData<T> : IBaseData<T>
    {
        private const string keyFieldNameConst = "_id";
        private const string dateCreatedFieldNameConst = "DateCreated";
        private const string dateModifiedFieldNameConst = "DateModified";

        protected Type collectionType { get; set; }
        protected string collectionName { get; set; }


        private string connectionString { get; set; }
        private string defaultDatabaseName { get; set; }

        private MongoDBConfig mongoDBConfig { get; set; }

        public BaseData(IConfiguration Configuration)
        {
            this.connectionString = ConnectionString;
            this.defaultDatabaseName = DefaultDatabaseName;

            this.collectionName = typeof(T).Name;

            this.mongoDBConfig = new MongoDBConfig(connectionString, defaultDatabaseName);
        }

        public ObjectId Save(T ObjectToSave)
        {
            ObjectId newId = new Helper(this.mongoDBConfig).Save(this.collectionName, ObjectToSave);

            PropertyInfo idProperty = typeof(T).GetProperty(keyFieldNameConst);
            idProperty.SetValue(ObjectToSave, newId);

            return newId;
        }


        #region Get All

        public List<T> GetAll()
        {
            List<T> list = Helper.GetAll<T>(collectionName);
            return list;
        }

        public List<T> GetList(List<Filter> filter, bool UseAndLogic = true)
        {
            //List<T> foundItems = Helper.GetAllFiltered<T>(collectionName, filter, "", UseAndLogic);
            //return foundItems;

            return null;
        }

        public Dictionary<string, string> GetKeyValueList(List<Filter> filter, bool UseAndLogic = true)
        {
            string keyFieldName = keyFieldNameConst;
            string valueFieldName = string.Format("{0}Name", collectionName);

            Dictionary<string, string> dict;

            dict = GetKeyValueList(keyFieldName, valueFieldName, filter, UseAndLogic);

            return dict;
        }

        public Dictionary<string, string> GetKeyValueList(string KeyFieldName, string ValueFieldName, List<Filter> filter, bool UseAndLogic = true)
        {
            //List<T> foundItems = Helper.GetAllFiltered<T>(collectionName, filter, "", UseAndLogic);

            //PropertyInfo idProperty = typeof(T).GetProperty(KeyFieldName);
            //PropertyInfo nameProperty = typeof(T).GetProperty(ValueFieldName);

            //if (idProperty == null)
            //{
            //    throw new Exception(string.Format("Unable to reflect key property. please check entity \"{0}\" contains \"{1}\" property.", collectionName, KeyFieldName));
            //}

            //if (nameProperty == null)
            //{
            //    throw new Exception(string.Format("Unable to reflect name property. please check entity \"{0}\" contains \"{1}\" property.", collectionName, ValueFieldName));
            //}

            //Dictionary<string, string> dict = new Dictionary<string, string>();

            //foreach (T item in foundItems)
            //{
            //    string key = idProperty.GetValue(item).ToString();
            //    string value = nameProperty.GetValue(item).ToString();
            //    dict.Add(key, value);
            //}

            //return dict;

            return null;
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
            //T foundItem = Helper.GetSingle<T>(collectionName, Id);

            //return foundItem;

            throw new Exception("not implemented");

        }

        #endregion



        #region Delete

        public bool Delete(string Id)
        {
            ObjectId id = new ObjectId(Id);
            bool result = this.Delete(id);
            return result;
        }

        public bool Delete(ObjectId Id)
        {
            //bool result = Helper.DeleteSingle<T>(collectionName, Id);
            //return result;

            throw new Exception("not implemented");
        }

        #endregion
    }
}
