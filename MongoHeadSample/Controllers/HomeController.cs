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
        private IConfiguration _configuration;

        public HomeController(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Test()
        {
            MongoDBConfig config = new MongoDBConfig(
                _configuration[MongoDBConfig.KeyNameConnectionString],
                _configuration[MongoDBConfig.KeyNameDatabaseName]
                );

            MongoDBHelper helper = new MongoDBHelper(config, typeof(Test));

            Test test = new Test();
            test.AliveAndKicking = true;
            test.DateOfBirth = new DateTime(1976, 4, 19);
            test.Name = "Ozan Kutlu";
            test.Surname = "Bayram";
            test._DateUtcCreated = DateTime.Now;
            test._DateUtcModified = DateTime.Now;
            //test._IsActive = true;

            //MongoDBHelper method samples for Test entity

            //save object to db
            test._id = helper.Save(test);

            //save some json
            //Please note that at least one field other than id must match to a entity property to get deserialized properly while retreiving from DB. 
            //Otherwise you will get format exception when you try to get that document.
            string json = "{ 'foo' : 'Hakkı' }";
            MongoDB.Bson.BsonDocument someBsonDocument
                = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(json);
            ObjectId fooId = helper.Save(someBsonDocument);

            //GetList<T>
            List<Test> foundItems1 = helper.GetList<Test>();

            //Get List by filter
            List<Filter> filterByName1 = new List<Filter>()
            {
                new Filter { PropertyName = "Name", Operation = Op.Equals, Value = "Hakkı" }
            };

            List<Test> foundItems2 = helper.GetList<Test>(filterByName1);

            //get item by filter
            List<Filter> filterByName2 = new List<Filter>()
            {
                new Filter { PropertyName = "Name", Operation = Op.Equals, Value = "Ozan Kutlu" }
            };
            Test foundItem1 = helper.Get<Test>(filterByName2);














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
