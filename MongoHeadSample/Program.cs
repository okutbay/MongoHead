using MongoHeadSample;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// This is required for AJAX requests to validate token in "__RequestVerificationToken" hidden input
builder.Services.AddAntiforgery(options => options.HeaderName = "XSRF-TOKEN");

var app = builder.Build();

#region Configuration

//[OKB] option 1 to get configuration
ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

//[OKB] accessing the values 
//var ConnectionString = configuration.GetSection("Settings:MongoDB:ConnectionString").Value;
//var DatabaseName = configuration.GetSection("Settings:MongoDB:DatabaseName").Value;

//[OKB] option 2 to get configuration
//IConfiguration configuration2 = app.Configuration;
//IWebHostEnvironment environment2 = app.Environment;

//[OKB] option 3 to get configuration
FabrikafaSettings fabrikafaSettings = builder.Configuration.Get<FabrikafaSettings>();
//TODO: we cannot find a way to bind this property with the names have dot in it. So we are assigning it manually
fabrikafaSettings.Logging.LogLevel.Microsoft_AspNetCore = configuration.GetSection("Logging:LogLevel:Microsoft.AspNetCore").Value;
string connStr = fabrikafaSettings.Settings.MongoDB.ConnectionString;

#endregion

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//        name: "default",
//        pattern: "{controller=Home}/{action=Index}/{id?}");
//    endpoints.MapRazorPages();
//});

app.UseAuthorization();

app.MapRazorPages();

app.Run();
