using MongoHead;
using MongoHeadSample.Models;

namespace MongoHeadSample.Data;

public class PersonData: BaseData<Person>
{
    private readonly IConfiguration _configuration;

    string collectionName = string.Empty;

    public PersonData(IConfiguration configuration): base(configuration)
    {
        _configuration = configuration;

        collectionName = base.CollectionName;
    }
}

