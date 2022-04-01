using Weather.API.Extensions;
using Weather.Persistent.Extensions;
using BootstrapExtensions = Weather.API.Extensions.BootstrapExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureMongoClient();
builder.Services.ConfigureRepositories();
builder.Services.ConfigureWeatherOfDayGeneratorService();

builder.Services.ConfigureCors();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(BootstrapExtensions.MyAllowSpecificOrigins);
app.CreateDbIfNoExist();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();