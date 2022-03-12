using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using MongoHeadSample.Business;
using MongoHeadSample.Models;
using MongoHeadSample.ViewModels;

namespace MongoHeadSample.Pages.People;

public class IndexModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<IndexModel> _logger;

    private readonly FabrikafaSettings fabrikafaSettings;

    string connectionString = string.Empty;
    string databaseName = string.Empty;
    string collectionName = string.Empty;

    public List<Person> PersonList { get; set; }

    public PersonViewModel PersonViewModel { get; set; }

    public IndexModel(IConfiguration configuration, ILogger<IndexModel> logger)
    {
        _logger = logger;
        _configuration = configuration;
        PersonList = new List<Person>();

        fabrikafaSettings = _configuration.Get<FabrikafaSettings>();

        //connectionString = _configuration["Settings:MongoDB:ConnectionString"];
        //databaseName = _configuration["Settings:MongoDB:DatabaseName"];

        connectionString = fabrikafaSettings.Settings.MongoDB.ConnectionString;
        databaseName = fabrikafaSettings.Settings.MongoDB.DatabaseName;
        collectionName = "People";//In the MongoHead way we don't need collection names. Collection names come from entinty class names and derived from BaseData class.
    }

    public async void OnGetAsync()
    {
        ////classic way
        //var client = new MongoClient(connectionString);
        //var db = client.GetDatabase(databaseName);
        //var collection = db.GetCollection<m.Person>(collectionName);

        //var person1 = new m.Person { FirstName = "Some", LastName = "Person", Age = 40 };

        //await collection.InsertOneAsync(person1);

        //var results = await collection.FindAsync(_ => true); //return every record
        //var resultList = results.ToList();

        ////MongoHead way
        //var person2 = new m.Person { FirstName = "SomeOther", LastName = "Person", Age = 45 };
        //PersonBusiness personBusiness = new PersonBusiness(_configuration);
        //personBusiness.AddNewPerson(person2);

        await Task.CompletedTask;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("Save error!", "Unable to save!");
            return Page();
        }

        await Task.CompletedTask;

        return Page();
    }

    public JsonResult OnPostDeletePerson([FromBody] PersonViewModel obj)
    {
        bool result = false;
        PersonBusiness personBusiness = new PersonBusiness(_configuration);
        Person foundItem = personBusiness.GetPerson(obj._id);

        if (foundItem != null)
        {
            personBusiness.DeletePerson(foundItem);
            result = true;
        }
        else
        {
            return new JsonResult("Person not found");
        }

        return new JsonResult(result);
    }
}