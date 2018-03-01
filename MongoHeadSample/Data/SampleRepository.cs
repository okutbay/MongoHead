using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;
using MongoDB.Driver;

using MongoHeadSample.Interfaces;
using MongoHeadSample.Models;
using MongoDB.Bson;

namespace MongoHeadSample.Data
{
    public class SampleRepository : ISampleRepository
    {
        private readonly SampleContext _context = null;

        public SampleRepository(IOptions<AppSettings> appSettings)
        {
            _context = new SampleContext(appSettings);
        }

        public async Task<IEnumerable<Sample>> GetAllSamples()
        {
            try
            {
                return await _context.Samples.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        // query after internal or internal id
        //
        public async Task<Sample> GetSample(string id)
        {
            try
            {
                ObjectId internalId = GetInternalId(id);

                return await _context.Samples
                                .Find(sample => sample.FriendlyId == id || sample._id == internalId)
                                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        private ObjectId GetInternalId(string id)
        {
            ObjectId internalId;
            if (!ObjectId.TryParse(id, out internalId))
                internalId = ObjectId.Empty;

            return internalId;
        }

        public async Task AddSample(Sample item)
        {
            try
            {
                await _context.Samples.InsertOneAsync(item);
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<bool> RemoveSample(string id)
        {
            try
            {
                DeleteResult actionResult = await _context.Samples.DeleteOneAsync(
                     Builders<Sample>.Filter.Eq("Id", id));

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<bool> UpdateSample(string id, string body)
        {
            var filter = Builders<Sample>.Filter.Eq(s => s.FriendlyId, id);
            var update = Builders<Sample>.Update
                            .Set(s => s.Content, body)
                            .CurrentDate(s => s.ModifyDate);

            try
            {
                UpdateResult actionResult = await _context.Samples.UpdateOneAsync(filter, update);

                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<bool> UpdateSample(string id, Sample item)
        {
            try
            {
                ReplaceOneResult actionResult = await _context.Samples
                                                .ReplaceOneAsync(n => n.FriendlyId.Equals(id)
                                                                , item
                                                                , new UpdateOptions { IsUpsert = true });
                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        // Demo function - full document update
        public async Task<bool> UpdateSampleDocument(string id, string body)
        {
            var item = await GetSample(id) ?? new Sample();
            item.Content = body;
            item.ModifyDate = DateTime.Now;

            return await UpdateSample(id, item);
        }

        public async Task<bool> RemoveAllSamples()
        {
            try
            {
                DeleteResult actionResult = await _context.Samples.DeleteManyAsync(new BsonDocument());

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        // it creates a compound index (first using UserId, and then Body)
        // MongoDb automatically detects if the index already exists - in this case it just returns the index details
        public async Task<string> CreateIndex()
        {
            try
            {
                return await _context.Samples.Indexes
                                           .CreateOneAsync(Builders<Sample>
                                                                .IndexKeys
                                                                .Ascending(item => item.UserId)
                                                                .Ascending(item => item.Content));
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
    }
}
