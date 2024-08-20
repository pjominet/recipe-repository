using RecipeRepository.Web;

var builder = WebApplication.CreateBuilder(args);

var loggerFactory = LoggerFactory.Create(configure => configure.AddConsole());
var logger = loggerFactory.CreateLogger<Program>();

var startup = new Startup(builder.Configuration, logger);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

Startup.Configure(app);

app.Run();
