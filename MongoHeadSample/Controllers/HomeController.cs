using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
            //TODO BUNU BURADA YAPMAYALIM YAHU. GIDELIM KENDI ORNEK PROJESINDE YAPALIM


            List<Filter> filter = new List<Filter>()
            {
                new Filter { PropertyName = "ShortenedURL", Operation = Op.Equals, Value = "http://cut.lu/c87sft" }
            };

            MongoDBConfig config = new MongoDBConfig(
                _configuration["MongoDBConfig:ConnectionString"],
                _configuration["MongoDBConfig:DefaultDatabaseName"]
                );

            MongoDBHelper helper = new MongoDBHelper(config, typeof(Test));

            //Burada helper metodlarımızı bir test edelim


            Test foundItem = helper.Get<Test>(filter);



            //burada bir basedata metodlarımızı test edelim


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
