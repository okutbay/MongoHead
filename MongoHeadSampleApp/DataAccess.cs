using System.Collections.Generic;

using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoHeadSampleApp
{
    public class DataAccess
    {
        MongoClient client;
        IMongoDatabase database;

        

        public DataAccess()
        {
            client = new MongoClient("mongodb://localhost:27017");
            database = client.GetDatabase("FabrikafaTestDb");

            //var client = new MongoClient("mongodb://localhost:27017");
            //var database = client.GetDatabase("foo");

        }

        public IMongoCollection<Product> GetProducts()
        {
            var collection = database.GetCollection<Product>("Products");

            //await collection.InsertOneAsync(new BsonDocument("Name", "Jack"));

            //var list = await collection.Find(new BsonDocument("Name", "Jack"))
            //    .ToListAsync();

            //foreach (var document in list)
            //{
            //    Console.WriteLine(document["Name"]);
            //}

            return database.GetCollection<Product>("Products");
        }


        public Product GetProduct(ObjectId id)
        {
            var res = Query<Product>.EQ(p => p.Id, id);
            return _db.GetCollection<Product>("Products").FindOne(res);
        }

        public Product Create(Product p)
        {
            _db.GetCollection<Product>("Products").Save(p);
            return p;
        }

        public void Update(ObjectId id, Product p)
        {
            p.Id = id;
            var res = Query<Product>.EQ(pd => pd.Id, id);
            var operation = Update<Product>.Replace(p);
            _db.GetCollection<Product>("Products").Update(res, operation);
        }
        public void Remove(ObjectId id)
        {
            var res = Query<Product>.EQ(e => e.Id, id);
            var operation = _db.GetCollection<Product>("Products").Remove(res);
        }
    }
}
