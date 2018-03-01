using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Driver;
using Microsoft.Extensions.Options;
using MongoHeadSample.Models;

namespace MongoHeadSample.Data
{
    public class SampleContext
    {
        private readonly IMongoDatabase _database = null;

        public SampleContext(IOptions<AppSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);

            if (client != null)
                _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Sample> Samples
        {
            get
            {
                return _database.GetCollection<Sample>("Sample");
            }
        }
    }
}
