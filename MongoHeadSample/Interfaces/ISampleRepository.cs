using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoHeadSample.Models;

namespace MongoHeadSample.Interfaces
{
    public interface ISampleRepository
    {
        Task<IEnumerable<Sample>> GetAllSamples();
        Task<Sample> GetSample(string _id);

        Task AddSample(Sample item);
        Task<string> CreateIndex();

        Task<bool> UpdateSample(string _id, string Content);
        Task<bool> UpdateSampleDocument(string _id, string Content);

        Task<bool> RemoveAllSamples();
        Task<bool> RemoveSample(string _id);
    }
}
