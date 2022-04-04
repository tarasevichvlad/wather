using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Serilog;
using Weather.Persistent.Repositories;
using Weather.Persistent.Services;

namespace Weather.Persistent.Extensions;

public static class BootstrapExtensions
{
    public static MongoDbOptions ConfigureMongoOptions(this IServiceCollection services, IConfiguration configuration, ILogger logger)
    {
        var optionsSection = configuration.GetSection(MongoDbOptions.OptionName);
        services.AddOptions<MongoDbOptions>()
            .Bind(optionsSection)
            .ValidateDataAnnotations();

        var options = optionsSection.Get<MongoDbOptions>();

        logger.Information("Added mongo options: Connection string: {ConnectionString}", options.ConnectionString);

        return options;
    }
    
    public static IServiceCollection ConfigureMongoClient(this IServiceCollection services, MongoDbOptions options)
    {
        services.AddSingleton<IMongoClient>(new MongoClient(options.ConnectionString));

        return services;
    }

    public static async Task CreateDbWithStubDataIfNoExist(this IHost webApplication, MongoDbOptions mongoDbOptions)
    {
        var mongoClient = webApplication.Services.GetService<IMongoClient>()!;

        var databases = (await mongoClient.ListDatabaseNamesAsync()).ToList();

        if (databases.Exists(x => x.Equals(mongoDbOptions.DatabaseName))) return;

        await mongoClient.GetDatabase(mongoDbOptions.DatabaseName).CreateCollectionAsync(mongoDbOptions.CollectionName);

        await GenerateStubData(webApplication);
    }

    public static IServiceCollection ConfigureWeatherOfDayGeneratorService(this IServiceCollection services)
    {
        services.AddSingleton<IWeatherOfDayGeneratorService, WeatherOfDayGeneratorService>();

        return services;
    }

    private static async Task GenerateStubData(this IHost webApplication)
    {
        var weatherOfDayGeneratorService = webApplication.Services.GetService<IWeatherOfDayGeneratorService>()!;
        var weatherForecastRepository = webApplication.Services.GetService<IWeatherForecastRepository>()!;

        var generateWeatherOfDays = weatherOfDayGeneratorService.GenerateWeatherOfDays(10);

        await weatherForecastRepository.CreateBulkAsync(generateWeatherOfDays);
    }

    public static IServiceCollection ConfigureRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IWeatherForecastRepository, WeatherForecastRepository>();

        return services;
    }
}