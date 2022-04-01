using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Weather.Persistent.Repositories;
using Weather.Persistent.Services;

namespace Weather.Persistent.Extensions;

public static class BootstrapExtensions
{
    public static IServiceCollection ConfigureMongoClient(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(new MongoClient("mongodb://root:example@localhost:27017"));

        return services;
    }

    public static void CreateDbIfNoExist(this IHost webApplication)
    {
        var mongoClient = webApplication.Services.GetService<IMongoClient>()!;

        var databases = mongoClient.ListDatabaseNames().ToList();

        if (databases.Exists(x => x.Equals("WeatherForecast"))) return;

        mongoClient.GetDatabase("WeatherForecast").CreateCollection("Weather");

        GenerateStubData(webApplication);
    }

    public static IServiceCollection ConfigureWeatherOfDayGeneratorService(this IServiceCollection services)
    {
        services.AddSingleton<IWeatherOfDayGeneratorService, WeatherOfDayGeneratorService>();

        return services;
    }

    private static void GenerateStubData(this IHost webApplication)
    {
        var weatherOfDayGeneratorService = webApplication.Services.GetService<IWeatherOfDayGeneratorService>()!;
        var weatherForecastRepository = webApplication.Services.GetService<IWeatherForecastRepository>()!;

        var generateWeatherOfDays = weatherOfDayGeneratorService.GenerateWeatherOfDays(10);

        weatherForecastRepository.CreateBulk(generateWeatherOfDays);
    }

    public static IServiceCollection ConfigureRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IWeatherForecastRepository, WeatherForecastRepository>();

        return services;
    }
}