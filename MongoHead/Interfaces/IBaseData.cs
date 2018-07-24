using System;
using System.Collections.Generic;
using System.Text;

using MongoDB.Bson;

namespace MongoHead.Interfaces
{
    public interface IBaseData<T>
    {
        List<T> GetList();
        List<T> GetList(List<Filter> Filter, bool UseAndLogic = true);
        Dictionary<string, string> GetKeyValueList(List<Filter> Filter, bool UseAndLogic = true);
        Dictionary<string, string> GetKeyValueList(string KeyFieldName, string ValueFieldName, List<Filter> Filter, bool UseAndLogic = true);

        T GetById(string Id);
        T GetById(ObjectId Id);

        ObjectId Save(T ObjectToSave);

        bool Delete(string Id);
        bool Delete(ObjectId Id);
    }
}
