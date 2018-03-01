using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


using MongoHeadSample.Interfaces;
using MongoHeadSample.Models;

namespace MongoHeadSample.Controllers
{
    public class SystemController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private readonly ISampleRepository _sampleRepository;

        public SystemController(ISampleRepository sampleRepository)
        {
            _sampleRepository = sampleRepository;
        }

        // Call an initialization - /system/init
        public IActionResult Init()
        {
            _sampleRepository.RemoveAllSamples();
            var name = _sampleRepository.CreateIndex();

            _sampleRepository.AddSample(new Sample() { FriendlyId = "1", Content = "Some sample content 1", CreateDate = DateTime.Now, ModifyDate = DateTime.Now, UserId = 1 });
            _sampleRepository.AddSample(new Sample() { FriendlyId = "2", Content = "Some sample content 2", CreateDate = DateTime.Now, ModifyDate = DateTime.Now, UserId = 1 });
            _sampleRepository.AddSample(new Sample() { FriendlyId = "3", Content = "Some sample content 3", CreateDate = DateTime.Now, ModifyDate = DateTime.Now, UserId = 2 });
            _sampleRepository.AddSample(new Sample() { FriendlyId = "4", Content = "Some sample content 4", CreateDate = DateTime.Now, ModifyDate = DateTime.Now, UserId = 2 });

            ViewBag.ResultMessage = "SamplesDb was created and filled with samples";

            return View(); 
        }
    }
}