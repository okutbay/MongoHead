using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using MongoHeadSample.Business;
using MongoHeadSample.Models;
using MongoHeadSample.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace MongoHeadSample.Pages.People;

public class FormModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<IndexModel> _logger;

    private readonly FabrikafaSettings fabrikafaSettings;

    string connectionString = string.Empty;
    string databaseName = string.Empty;
    string collectionName = string.Empty;

    public FormModel(IConfiguration configuration, ILogger<IndexModel> logger)
    {
        _logger = logger;
        _configuration = configuration;
        Input = new InputModel();

        fabrikafaSettings = _configuration.Get<FabrikafaSettings>();
        connectionString = fabrikafaSettings.Settings.MongoDB.ConnectionString;
        databaseName = fabrikafaSettings.Settings.MongoDB.DatabaseName;
        collectionName = "People";//In the MongoHead way we don't need collection names. Collection names come from entinty class names and derived from BaseData class.
    }

    public class InputModel
    {
        public InputModel()
        {
            this.PersonViewModel = new PersonViewModel();
            this.OperationType = OperationTypeEnum.Add;
        }

        public PersonViewModel PersonViewModel { get; set; }
        public OperationTypeEnum OperationType { get; set; }

    }

    [BindProperty]
    public InputModel Input { get; set; }

    public IActionResult OnGet(string? personid)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("Save error!", "Unable to save! Please fix all validation error(s).");
            return Page();
        }

        if (string.IsNullOrEmpty(personid))
        {
            this.Input.OperationType = OperationTypeEnum.Add;
            this.Input.PersonViewModel = new PersonViewModel();
            this.Input.PersonViewModel._id = ObjectId.Empty.ToString();
        }
        else
        {
            this.Input.OperationType = OperationTypeEnum.Update;
        }

        switch (this.Input.OperationType)
        {
            case OperationTypeEnum.Add:
                break;
            case OperationTypeEnum.Update:
                PersonBusiness personBusiness = new PersonBusiness(_configuration);
                Person foundItem = personBusiness.GetPerson(personid);

                if (foundItem != null) {
                    //TODO: Will use automapper later
                    Input.PersonViewModel._id = foundItem._id.ToString();
                    Input.PersonViewModel.FirstName = foundItem.FirstName;
                    Input.PersonViewModel.LastName = foundItem.LastName;
                    Input.PersonViewModel.Age = foundItem.Age;
                }
                break;
            case OperationTypeEnum.Remove:
                break;
            default:
                break;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("Save error!", "Unable to save! Please fix all validation error(s).");
        }

        PersonViewModel PersonViewModel = this.Input.PersonViewModel;

        PersonBusiness personBusiness = new PersonBusiness(_configuration);
        Person item = personBusiness.GetPerson(PersonViewModel._id);

        if (item == null)
        {
            item = new Person();
        }

        //TODO: Will use automapper later
        //item._id = new ObjectId(Input.PersonViewModel._id);
        item.FirstName = Input.PersonViewModel.FirstName;
        item.LastName = Input.PersonViewModel.LastName;
        item.Age = Input.PersonViewModel.Age;

        personBusiness.AddUpdatePerson(item);



        await Task.CompletedTask;

        return Page();
    }
}
