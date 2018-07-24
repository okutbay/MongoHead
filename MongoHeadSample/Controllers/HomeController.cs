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

            Test test = new Test()
            {
                AliveAndKicking = false,
                DateOfBirth = new DateTime(1936, 1, 20),
                Name = "Ahmet",
                Surname = "Bayram",
                _DateUtcCreated = DateTime.Now,
                _DateUtcModified = DateTime.Now,
                _IsActive = false

            };
            helper.Save(test);

            test = new Test()
            {
                AliveAndKicking = false,
                DateOfBirth = new DateTime(1920, 3, 5),
                Name = "Hasan",
                Surname = "Bayram",
                _DateUtcCreated = DateTime.Now,
                _DateUtcModified = DateTime.Now,
                _IsActive = false
            };
            helper.Save(test);

            test = new Test()
            {
                AliveAndKicking = false,
                DateOfBirth = new DateTime(1950, 9, 26),
                Name = "Serol",
                Surname = "Bayram",
                _DateUtcCreated = DateTime.Now,
                _DateUtcModified = DateTime.Now,
                _IsActive = false
            };
            helper.Save(test);

            test = new Test()
            {
                AliveAndKicking = true,
                DateOfBirth = new DateTime(2008, 9, 16),
                Name = "Melis",
                Surname = "Bayram",
                _DateUtcCreated = DateTime.Now,
                _DateUtcModified = DateTime.Now,
                _IsActive = true
            };
            helper.Save(test);
        }

        public IActionResult Test()
        {
            //InitData();

            MongoDBConfig config = new MongoDBConfig(
                _configuration[MongoDBConfig.KeyNameConnectionString],
                _configuration[MongoDBConfig.KeyNameDatabaseName]
                );

            MongoDBHelper helper = new MongoDBHelper(config, typeof(Test));

            Test test = new Test()
            {
                AliveAndKicking = true,
                DateOfBirth = new DateTime(1976, 4, 19),
                Name = "Ozan Kutlu",
                Surname = "Bayram",
                _DateUtcCreated = DateTime.Now,
                _DateUtcModified = DateTime.Now,
                _IsActive = true
            };

            //MongoDBHelper method samples for Test entity

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

            //public List<T> GetList<T>(List<Filter> Filter) //Get List by filter
            List<Filter> filterByName1 = new List<Filter>()
            {
                new Filter { PropertyName = "Name", Operation = Op.Equals, Value = "Hakkı" }
            };

            List<Test> foundItems2 = helper.GetList<Test>(filterByName1);

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



            //BaseData method samples for Test entity





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
    }
}
