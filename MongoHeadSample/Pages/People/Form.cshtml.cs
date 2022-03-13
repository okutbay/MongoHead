using AutoMapper;
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
    public readonly IMapper _mapper;

    private readonly FabrikafaSettings fabrikafaSettings;

    string connectionString = string.Empty;
    string databaseName = string.Empty;
    string collectionName = string.Empty;

    public FormModel(IConfiguration configuration, ILogger<IndexModel> logger, IMapper mapper)
    {
        _logger = logger;
        _configuration = configuration;
        _mapper = mapper;

        Input = new InputModel();
        Message = string.Empty;

        fabrikafaSettings = _configuration.Get<FabrikafaSettings>();
        connectionString = fabrikafaSettings.Settings.MongoDB.ConnectionString;
        databaseName = fabrikafaSettings.Settings.MongoDB.DatabaseName;
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

    [TempData]
    public string Message { get; set; }

    public IActionResult OnGet(string? personid)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("Save error!", "Unable to save! Please fix all validation error(s).");
            return Page();
        }

        if (string.IsNullOrEmpty(personid))
        {
            personid = ObjectId.Empty.ToString();

            this.Input.OperationType = OperationTypeEnum.Add;
            this.Input.PersonViewModel = new PersonViewModel();
            this.Input.PersonViewModel._id = personid;
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
                    Input.PersonViewModel = _mapper.Map<Person, PersonViewModel>(foundItem);
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
            return Page();
        }

        PersonViewModel PersonViewModel = this.Input.PersonViewModel;

        PersonBusiness personBusiness = new PersonBusiness(_configuration);
        Person item = personBusiness.GetPerson(PersonViewModel._id);

        if (item == null)
        {
            item = new Person();
        }

        item = _mapper.Map<PersonViewModel, Person>(Input.PersonViewModel);

        personBusiness.AddUpdatePerson(item);

        Message = $"Operation {Input.OperationType.ToString()} completed successfuly for person <b>'{item.FullName}'</b>";

        await Task.CompletedTask;

        return Page();
    }
}