using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;

namespace MongoHead
{
    public class BaseData<T> : IBaseData<T>
    {
        private const string keyFieldNameConst = "_id";

        protected Type collectionType = null;
        protected string collectionName = string.Empty;

        public BaseData()
        {
            collectionType = this.GetType();
            collectionName = collectionType.Name;
        }

        public List<T> GetAll()
        {
            throw new NotImplementedException();
        }

        public List<T> GetList(List<Filter> filter, bool UseAndLogic = true)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetKeyValueList(List<Filter> filter, bool UseAndLogic = true)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetKeyValueList(string KeyFieldName, string ValueFieldName, List<Filter> filter, bool UseAndLogic = true)
        {
            throw new NotImplementedException();
        }

        public T GetById(string Id)
        {
            throw new NotImplementedException();
        }

        public T GetById(ObjectId Id)
        {
            throw new NotImplementedException();
        }

        public ObjectId Save(T ObjectToSave)
        {
            throw new NotImplementedException();
        }

        public bool Delete(string Id)
        {
            throw new NotImplementedException();
        }

        public bool Delete(ObjectId Id)
        {
            throw new NotImplementedException();
        }
    }
}
