using Microsoft.OpenApi.Models;
using I18NPuzzles.Gateways;
using I18NPuzzles.PuzzleHelper;
using I18NPuzzles.Services;
using I18NPuzzles.Controllers;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "I18NPuzzles", Version = "v1" });
    c.ParameterFilter<ParameterFilter>();

    string filePath = Path.Combine(AppContext.BaseDirectory, "WebAPI.xml");
    c.IncludeXmlComments(filePath);
});

// Add the gateway as singleton since almost all API calls use it and it sets up a client that we'd like to keep configured
builder.Services.AddSingleton<I18NPuzzlesGateway>();

// Adding all services as Transient because on each request, we should only call the service once.
// We could use Singleton for performance improvement on successive calls,
//    but because we want to avoid spamming the I18N Puzzles server, we'll assume that this performance is negligible.
builder.Services.AddTransient<SolutionService>();
builder.Services.AddTransient<PuzzleHelperService>();

#region Setup Daily Solution Services
// Get a list of assembly types for the whole app
IEnumerable<Type> assemblyTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes());

// Get only the types for the classes that inherit from the ISolutionDayService
IEnumerable<Type> solutionDayServiceTypes = assemblyTypes.Where(x => !x.IsInterface && x.GetInterface(nameof(ISolutionDayService)) != null);

// Register each Solution Day Service class
foreach (Type solutionDayServiceType in solutionDayServiceTypes)
{
    // This is not null because of the filter a few lines above
    Type interfaceType = solutionDayServiceType.GetInterface(nameof(ISolutionDayService))!;
    
    builder.Services.AddTransient(interfaceType, solutionDayServiceType);
}
#endregion

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();