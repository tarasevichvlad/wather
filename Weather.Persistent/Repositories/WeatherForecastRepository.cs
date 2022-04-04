using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Weather.Contracts;
using Weather.Persistent.Abstractions;

namespace Weather.Persistent.Repositories;

/// <summary>
/// Предоставляет интерфейс работы с базой данных.
/// Репозиторий позволяет работать с погодой.
/// </summary>
public interface IWeatherForecastRepository : IRepository<WeatherOfDay>, IBulkRepository<WeatherOfDay>
{
    Task<IEnumerable<WeatherOfDay>> GetWeatherOfDaysByCityNameAsync(string name, string dateTime);
    Task<IEnumerable<string>> GetCountryNamesAsync();
}

public class WeatherForecastRepository : IWeatherForecastRepository
{
    private readonly IMongoCollection<WeatherOfDay> _collection;

    public WeatherForecastRepository(IMongoClient mongoClient, IOptions<MongoDbOptions> mongoDbOptions)
    {
        
        _collection = mongoClient.GetDatabase(mongoDbOptions.Value.DatabaseName).GetCollection<WeatherOfDay>(mongoDbOptions.Value.CollectionName);
    }

    public async Task<WeatherOfDay> GetItemAsync(Guid id)
    {
        var result = await _collection.FindAsync(x => x.CityName.Equals(id));
        return await result.SingleAsync();
    }

    public async Task CreateAsync(WeatherOfDay weatherOfDay)
    {
        await _collection.InsertOneAsync(weatherOfDay);
    }

    public async Task CreateBulkAsync(IEnumerable<WeatherOfDay> weatherOfDay)
    {
        await _collection.InsertManyAsync(weatherOfDay);
    }

    public async Task<IEnumerable<WeatherOfDay>> GetWeatherOfDaysByCityNameAsync(string name, string dateTime)
    {
        var result = await _collection.FindAsync(x => x.CityName == name && x.Date.Equals(dateTime));
        return await result.ToListAsync();
    }

    public async Task<IEnumerable<string>> GetCountryNamesAsync()
    {
        var result = await _collection.DistinctAsync(day => day.CityName, day => true);
        return await result.ToListAsync();
    }
}