using Serilog;
using Weather.API.Extensions;
using Weather.Persistent.Extensions;
using Weather.Tools.Parser;
using BootstrapExtensions = Weather.API.Extensions.BootstrapExtensions;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.AddSerilog(logger);

var options = builder.Services.ConfigureMongoOptions(builder.Configuration, logger);

builder.Services
    .ConfigureMongoClient(options)
    .ConfigureRepositories()
    .ConfigureWeatherOfDayGeneratorService()
    .ConfigureParser()
    .ConfigureCors();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(BootstrapExtensions.AnyOrigins);
await app.CreateDbWithRealDataIfNoExist(options);

app.UseAuthorization();

app.MapControllers();

app.Run();