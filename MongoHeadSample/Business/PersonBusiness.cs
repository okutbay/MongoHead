using MongoDB.Bson;
using MongoHeadSample.Data;
using MongoHeadSample.Models;

namespace MongoHeadSample.Business;
public class PersonBusiness
{
    private readonly IConfiguration _configuration;
    private PersonData personData;

    public PersonBusiness(IConfiguration configuration)
    {
        _configuration = configuration;
        personData = new PersonData(_configuration);
    }

    /// <summary>
    /// Get list of all Persons
    /// </summary>
    public List<Person> GetAllPersons()
    {
        List<Person> foundItems = personData.GetList();

        return foundItems;
    }

    /// <summary>
    /// Get person with given id
    /// </summary>
    /// <param name="PersonId"></param>
    /// <returns></returns>
    public Person GetPerson(ObjectId PersonId)
    {
        return personData.GetById(PersonId);
    }

    /// <summary>
    /// Get person with given id
    /// </summary>
    /// <param name="PersonId"></param>
    /// <returns></returns>
    public Person GetPerson(string PersonId)
    {
        return personData.GetById(PersonId);
    }

    /// <summary>
    /// Adds a new person to Persons or updates the person
    /// </summary>
    /// <param name="person"></param>
    public void AddUpdatePerson(Person person)
    {
        personData.Save(person);
    }


    /// <summary>
    /// Searchs and returns list of found Persons
    /// </summary>
    /// <param name="searchText"></param>
    public List<Person> SearchPerson(string searchText)
    {
        List<Person> foundItems = personData.GetList();

        return foundItems;
    }

    /// <summary>
    /// Deletes the person from Persons
    /// </summary>
    /// <param name="person"></param>
    public void DeletePerson(Person person)
    {
        personData.Delete(person._id);
    }

    /// <summary>
    /// Seeds the Person collection
    /// </summary>
    public void Seed()
    {
        Person person = new Person() {
            FirstName = "Ozan Kutlu",
            LastName = "Bayram",
            Age = 46
        };
        this.AddUpdatePerson(person);

        person = new Person()
        {
            FirstName = "Burcu",
            LastName = "Bayram",
            Age = 34
        };
        this.AddUpdatePerson(person);

        person = new Person()
        {
            FirstName = "Melis",
            LastName = "Bayram",
            Age = 14
        };
        this.AddUpdatePerson(person);

        person = new Person()
        {
            FirstName = "Ahmet",
            LastName = "Kara",
            Age = 3
        };
        this.AddUpdatePerson(person);
    }

    /// <summary>
    /// Creates the index for person table to remove documents older than 2 days
    /// </summary>
    public async void CreateIndexAsync_ResetTableT2Days()
    {
        string indexName = "ResetPersonTableT2Days";
        TimeSpan duration = TimeSpan.FromDays(2);
        try
        {
            await personData.CreateIndexExpireAfterDuration(indexName, duration);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
