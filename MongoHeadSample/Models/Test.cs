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
    public class TestBusiness
    {
        readonly IConfiguration _configuration;

        public TestBusiness(IConfiguration Configuration)
        {
            this._configuration = Configuration;
        }

        public List<Test> GetBySurname(string Surname)
        {
            List<Filter> filter = new List<Filter>()
            {
                new Filter { PropertyName = "Surname", Operation = Op.Equals, Value = Surname }
            };

            TestData testData = new TestData(_configuration);

            List<Test> foundItem = testData.GetList(filter);

            return foundItem;
        }
    }

    //DATA LAYER
    internal class TestData : MongoHead.BaseData<Test>
    {
        //TODO Which one is good using generic type T or explicit type like Test
        //imho explicit type

        readonly IConfiguration _configuration;

        string CollectionName { get; set; }

        public TestData(IConfiguration Configuration) : base(Configuration)
        {
            this._configuration = Configuration;
            this.CollectionName = typeof(Test).Name;
        }
    }

    //ENTITY DEFINITION
    [BsonIgnoreExtraElements]
    public class Test : BaseEntity
    {
        public Test() : base()
        { }

        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool AliveAndKicking { get; set; }
    }




}
