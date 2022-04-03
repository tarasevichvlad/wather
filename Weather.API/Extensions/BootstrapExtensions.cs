using MongoDB.Driver;
using Weather.Persistent;
using Weather.Tools.Parser;

namespace Weather.API.Extensions;

public static class BootstrapExtensions
{
    public const string AnyOrigins = "AnyOrigins";

    public static IServiceCollection ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(AnyOrigins,
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        return services;
    }

    public static async Task CreateDbWithRealDataIfNoExist(this IHost webApplication, MongoDbOptions mongoDbOptions)
    {
        var mongoClient = webApplication.Services.GetService<IMongoClient>()!;
        var parser = webApplication.Services.GetService<IParser>()!;

        var databases = (await mongoClient.ListDatabaseNamesAsync()).ToList();

        if (databases.Exists(x => x.Equals(mongoDbOptions.DatabaseName))) return;

        await parser.ParseAsync();
    }
}