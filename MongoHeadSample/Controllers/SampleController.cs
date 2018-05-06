using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


using MongoHeadSample.Interfaces;
using MongoHeadSample.Models;

namespace MongoHeadSample.Controllers
{
    public class SampleController : Controller
    {
        private readonly ISampleRepository _sampleRepository;

        public SampleController(ISampleRepository sampleRepository)
        {
            _sampleRepository = sampleRepository;
        }

        public async Task<IActionResult> Index(string id)
        {
            //get the collection with specified id
            string itemId = id ?? "1";

            Sample sample = await _sampleRepository.GetSample(itemId) ?? new Sample();
            ViewData["Message"] = string.Format($"Sample Id: {itemId} - Content: {sample.Content}");

            return View();
        }
    }
}