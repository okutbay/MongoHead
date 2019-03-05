using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoHead;
using MongoHeadSample.Models;

namespace MongoHeadSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        private void InitData()
        {
            MongoDBConfig config = new MongoDBConfig(
                _configuration[MongoDBConfig.KeyNameConnectionString],
                _configuration[MongoDBConfig.KeyNameDatabaseName]
                );

            MongoDBHelper helper = new MongoDBHelper(config, typeof(Test));

            ObjectId someObjectIdValue = new ObjectId("5c6e82d721654c50981bddec");

            Test test = new Test()
            {
                AliveAndKicking = false,
                DateOfBirth = new DateTime(1936, 1, 20),
                Name = "Ahmet",
                Surname = "Bayram",
                _DateUtcCreated = DateTime.Now,
                _DateUtcModified = DateTime.Now,
                _IsActive = false,
                SomeObjectIdValue = someObjectIdValue

            };
            helper.Insert(test);

            test = new Test()
            {
                AliveAndKicking = false,
                DateOfBirth = new DateTime(1920, 3, 5),
                Name = "Hasan",
                Surname = "Bayram",
                _DateUtcCreated = DateTime.Now,
                _DateUtcModified = DateTime.Now,
                _IsActive = false,
                SomeObjectIdValue = someObjectIdValue
            };
            helper.Insert(test);

            test = new Test()
            {
                AliveAndKicking = false,
                DateOfBirth = new DateTime(1950, 9, 26),
                Name = "Serol",
                Surname = "Bayram",
                _DateUtcCreated = DateTime.Now,
                _DateUtcModified = DateTime.Now,
                _IsActive = false,
                SomeObjectIdValue = someObjectIdValue
            };
            helper.Insert(test);

            test = new Test()
            {
                AliveAndKicking = true,
                DateOfBirth = new DateTime(2008, 9, 16),
                Name = "Melis",
                Surname = "Bayram",
                _DateUtcCreated = DateTime.Now,
                _DateUtcModified = DateTime.Now,
                _IsActive = true,
                SomeObjectIdValue = someObjectIdValue
            };
            helper.Insert(test);
        }

        private void InitParameters()
        {
            MongoDBConfig config = new MongoDBConfig(
                _configuration[MongoDBConfig.KeyNameConnectionString],
                _configuration[MongoDBConfig.KeyNameDatabaseName]
                );

            MongoDBHelper helper = new MongoDBHelper(config, typeof(Parameter));

            Parameter parameter = new Parameter()
            {
                ParameterName = "Türkçe",
                ParameterValue = "tr",
                GroupName = "Languages",
                _DateUtcCreated = DateTime.Now,
                _DateUtcModified = DateTime.Now,
                _IsActive = true
            };

            Parameter foundParameter = helper.GetByFieldValue<Parameter>("ParameterName", "Türkçe");
            if (foundParameter == null)
            {
                helper.Insert(parameter);
            }

            parameter = new Parameter()
            {
                ParameterName = "English",
                ParameterValue = "en",
                GroupName = "Languages",
                _DateUtcCreated = DateTime.Now,
                _DateUtcModified = DateTime.Now,
                _IsActive = true
            };

            foundParameter = helper.GetByFieldValue<Parameter>("ParameterName", "English");
            if (foundParameter == null)
            {
                helper.Insert(parameter);
            }

        }

        public IActionResult Init()
        {
            InitData();
            InitParameters();

            return View();
        }

        public IActionResult Test()
        {
            MongoDBConfig config = new MongoDBConfig(
                _configuration[MongoDBConfig.KeyNameConnectionString],
                _configuration[MongoDBConfig.KeyNameDatabaseName]
                );

            ObjectId someObjectIdValue = new ObjectId("5c6e82d721654c50981bddec");

            MongoDBHelper helper = new MongoDBHelper(config, typeof(Test));

            Test test = new Test()
            {
                AliveAndKicking = true,
                DateOfBirth = new DateTime(1976, 4, 19),
                Name = "Ozan Kutlu",
                Surname = "Bayram",
                _DateUtcCreated = DateTime.Now,
                _DateUtcModified = DateTime.Now,
                _IsActive = true,
                SomeObjectIdValue = someObjectIdValue
            };

            //***********************************************************************************************
            //MongoDBHelper method samples for Test entity
            //***********************************************************************************************

            //save object to db
            //test._id = helper.Save(test);

            //save some json
            //Please note that at least one field other than id must match to a entity property to get deserialized properly while retreiving from DB. 
            //Otherwise you will get format exception when you try to get that document.
            string json = "{ 'Name' : 'Hakkı' }";
            MongoDB.Bson.BsonDocument someBsonDocument
                = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(json);
            //ObjectId fooId = helper.Save(someBsonDocument);

            //public List<T> GetList<T>()
            List<Test> foundItems1 = helper.GetList<Test>();

            //public List<T> GetList<T>(ObjectId Key, string KeyFieldName)
            List<Test> foundItems2 = helper.GetList<Test>(someObjectIdValue, "SomeObjectIdValue");

            //public List<T> GetList<T>(List<Filter> Filter) //Get List by filter
            List<Filter> filterByName1 = new List<Filter>()
            {
                new Filter { PropertyName = "Name", Operation = Op.Equals, Value = "Hakkı" }
            };

            List<Test> foundItems3 = helper.GetList<Test>(filterByName1);

            //public T Get<T>(List<Filter> Filter) //get item by filter
            List<Filter> filterByName2 = new List<Filter>()
            {
                new Filter { PropertyName = "Name", Operation = Op.Equals, Value = "Ozan Kutlu" }
            };
            Test foundItem1 = helper.Get<Test>(filterByName2);


            //public T GetByObjectId<T>(ObjectId _id) //get item by filter
            ObjectId _id = new ObjectId("5b56cb0025e1ee0d38fdbc26");
            Test foundItem2 = helper.GetByObjectId<Test>(_id);


            //public T GetByFieldValue<T>(string FieldName, object Value) //
            Test foundItem3 = helper.GetByFieldValue<Test>("Surname", "Bayram");

            //public T GetLast<T>(string SortFieldName) //
            Test foundItem4 = helper.GetLast<Test>("Surname");

            //public T GetLast<T>(List<Filter> Filter, string SortFieldName) //
            List<Filter> filterByName3 = new List<Filter>()
            {
                new Filter { PropertyName = "Name", Operation = Op.Equals, Value = "Hasan" }
            };
            Test foundItem5 = helper.GetLast<Test>(filterByName3, "Surname");


            //public T GetLast<T>(List<Filter> Filter, string SortFieldName) //
            List<Filter> filterByName4 = new List<Filter>()
            {
                new Filter { PropertyName = "Name", Operation = Op.Equals, Value = "Ahmet" }
            };
            Test foundItem6 = helper.GetLast<Test>(filterByName4, "Surname", false);


            //public bool Delete<T>(ObjectId _id) //
            Test testDataToDelete = new Test()
            {
                AliveAndKicking = false,
                DateOfBirth = new DateTime(1901, 1, 31),
                Name = "Del",
                Surname = "Ete",
                _DateUtcCreated = DateTime.Now,
                _DateUtcModified = DateTime.Now,
                _IsActive = true
            };
            testDataToDelete._id = helper.Insert(testDataToDelete);

            helper.Delete<Test>(testDataToDelete._id);




            //***********************************************************************************************
            //BaseData method samples for Test entity
            //***********************************************************************************************



            //create base data instance
            BaseData<Test> testBaseData = new BaseData<Test>(_configuration);
            BaseData<Parameter> parameterBaseData = new BaseData<Parameter>(_configuration);

            //public ObjectId Save(T ObjectToSave)
            testDataToDelete._id = testBaseData.Save(testDataToDelete);

            //public bool Delete(string Id) //
            bool deleteResult = testBaseData.Delete(testDataToDelete._id.ToString());


            testDataToDelete._id = testBaseData.Save(testDataToDelete);

            //public bool Delete(ObjectId Id) //
            bool deleteResult2 = testBaseData.Delete(testDataToDelete._id);


            //public List<T> GetList()
            testBaseData.GetList();


            //public List<T> GetList(List<Filter> filter, bool UseAndLogic = true) //
            List<Filter> filterByName5 = new List<Filter>()
            {
                new Filter { PropertyName = "Name", Operation = Op.Equals, Value = "Ahmet" }
            };
            testBaseData.GetList(filterByName5, false);

            //public Dictionary<string, string> GetKeyValueList(string GroupName, bool UseAndLogic = true) //
            List<Filter> filterByGroupName = new List<Filter>()
            {
                new Filter { PropertyName = "GroupName" , Operation = Op.Equals, Value = "Languages" }
            };
            Dictionary<string, string> languageList = parameterBaseData.GetKeyValueList(filterByGroupName);

            //public Dictionary<string, string> GetKeyValueList(string KeyFieldName, string ValueFieldName, List<Filter> filter, bool UseAndLogic = true) //
            Dictionary<string, string> languageList2 = parameterBaseData.GetKeyValueList("ParameterName", "ParameterValue", filterByGroupName);

            //public T GetById(string Id) //
            Test Melis = testBaseData.GetById("5b56cb0025e1ee0d38fdbc27");

            //public T GetById(ObjectId Id)
            Test Melis2 = testBaseData.GetById(new ObjectId("5b56cb0025e1ee0d38fdbc27"));



            //public ObjectId Save(T ObjectToSave)

            return View();
        }


        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult DeleteByFieldValue()
        {
            ViewData["Message"] = "Your application description page.";

            MongoDBConfig config = new MongoDBConfig(
                _configuration[MongoDBConfig.KeyNameConnectionString],
                _configuration[MongoDBConfig.KeyNameDatabaseName]
                );

            ObjectId someObjectIdValue = new ObjectId("5c6e82d721654c50981bddec");

            Test test = new Test()
            {
                AliveAndKicking = true,
                DateOfBirth = new DateTime(1976, 4, 19),
                Name = "Ozan Kutlu",
                Surname = "Bayram",
                _DateUtcCreated = DateTime.Now,
                _DateUtcModified = DateTime.Now,
                _IsActive = true,
                SomeObjectIdValue = someObjectIdValue
            };

            MongoDBHelper helper = new MongoDBHelper(config, typeof(Parameter));

            helper.Insert(test);

            long deletedCount = 0;
            bool deleteResult = helper.DeleteByFieldValue<Test>("SomeObjectIdValue", someObjectIdValue, out deletedCount);

            ViewData["DeletedCount"] = deletedCount.ToString();
            ViewData["DeleteResult"] = deleteResult.ToString();

            return View();
        }
        
    }
}
