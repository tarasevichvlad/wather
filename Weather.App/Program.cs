using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Refit;
using Serilog;
using Weather.App;
using Weather.App.Apis;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.RollingFile(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
    .WriteTo.File($"{Directory.GetCurrentDirectory()}/log-{DateTime.Now.ToShortDateString()}.log")
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Configuration.AddEnvironmentVariables();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
var uriFromSettings = builder.Configuration.GetSection("WeatherApiOptions").GetValue<string>("Url");
var uriEnvironmentVariable = Environment.GetEnvironmentVariable("WeatherApiOptions__Url");
var uri = uriEnvironmentVariable ?? uriFromSettings ?? string.Empty;
logger.Information("Uri From Settings: {Uri}",uriFromSettings);
logger.Information("Uri Environment Variable: {Uri}",uriEnvironmentVariable);
logger.Information("Uri result: {Uri}",uri);
builder.Services
    .AddRefitClient<IWeatherApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri));

builder.Services.AddMudServices();


await builder.Build().RunAsync();