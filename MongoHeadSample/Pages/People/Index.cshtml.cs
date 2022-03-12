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

    public IndexModel(IConfiguration configuration, ILogger<IndexModel> logger)
    {
        _logger = logger;
        _configuration = configuration;
        PersonList = new List<Person>();

        fabrikafaSettings = _configuration.Get<FabrikafaSettings>();
        connectionString = fabrikafaSettings.Settings.MongoDB.ConnectionString;
        databaseName = fabrikafaSettings.Settings.MongoDB.DatabaseName;
    }

    public async void OnGetAsync()
    {
        PersonBusiness personBusiness = new PersonBusiness(_configuration);
        PersonList = personBusiness.GetAllPersons();

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