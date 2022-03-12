using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using MongoHeadSample.Business;
using m = MongoHeadSample.Models;

namespace MongoHeadSample.Pages;

public class IndexModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<IndexModel> _logger;

    private readonly FabrikafaSettings fabrikafaSettings;

    public IndexModel(IConfiguration configuration, ILogger<IndexModel> logger)
    {
        _logger = logger;
        _configuration = configuration;

        fabrikafaSettings = _configuration.Get<FabrikafaSettings>();

        ConnectionString = fabrikafaSettings.Settings.MongoDB.ConnectionString;
        DatabaseName = fabrikafaSettings.Settings.MongoDB.DatabaseName;
    }

    public string ConnectionString { get; set; }

    public string DatabaseName { get; set; }

    public void OnGet()
    {
    }
}