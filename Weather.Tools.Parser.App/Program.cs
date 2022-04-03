using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Weather.Persistent;
using Weather.Persistent.Repositories;
using Weather.Tools.Parser;

var options = new MongoDbOptions
{
    ConnectionString = "mongodb://root:example@localhost:27017",
    CollectionName = "Weather",
    DatabaseName = "WeatherForecast"
};

var mongoClient = new MongoClient(options.ConnectionString);

var repository = new WeatherForecastRepository(mongoClient, Options.Create(options));

var parser = new Parser(repository);

await parser.ParseAsync();