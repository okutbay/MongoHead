using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization.Attributes;
using MongoHead;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoHeadSample.Models
{
    //BUSINESS LAYER
    public class ParameterBusiness
    {
        readonly IConfiguration _configuration;

        public ParameterBusiness(IConfiguration Configuration)
        {
            this._configuration = Configuration;
        }

        public List<Parameter> GetBySurname(string Surname)
        {
            List<Filter> filter = new List<Filter>()
            {
                new Filter { PropertyName = "Surname", Operation = Op.Equals, Value = Surname }
            };

            ParameterData parameterData = new ParameterData(_configuration);

            List<Parameter> foundItem = parameterData.GetList(filter);

            return foundItem;
        }
    }

    //DATA LAYER
    internal class ParameterData : MongoHead.BaseData<Parameter>
    {
        //TODO Which one is good using generic type T or explicit type like Test
        //imho explicit type

        readonly IConfiguration _configuration;

        string CollectionName { get; set; }

        public ParameterData(IConfiguration Configuration) : base(Configuration)
        {
            this._configuration = Configuration;
            this.CollectionName = typeof(Parameter).Name;
        }
    }

    //ENTITY DEFINITION
    [BsonIgnoreExtraElements]
    public class Parameter : BaseEntity
    {
        public Parameter() : base()
        { }

        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public string GroupName { get; set; }
    }




}
