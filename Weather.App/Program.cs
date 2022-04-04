using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Refit;
using Serilog;
using Weather.App;
using Weather.App.Apis;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Configuration.AddEnvironmentVariables();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
var uri = builder.Configuration.GetSection("WeatherApiOptions").GetValue<string>("Url");
builder.Services
    .AddRefitClient<IWeatherApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(uri));

builder.Services.AddMudServices();

await builder.Build().RunAsync();