using MongoDB.Driver;
using Weather.Contracts;
using Weather.Persistent.Abstractions;

namespace Weather.Persistent.Repositories;

public interface IWeatherForecastRepository : IRepository<WeatherOfDay>, IBulkRepository<WeatherOfDay>
{
    IEnumerable<WeatherOfDay> GetWeatherOfDaysByCityName(string name, string dateTime);
    IEnumerable<string> GetCountryNames();
}

public class WeatherForecastRepository : IWeatherForecastRepository
{
    private readonly IMongoCollection<WeatherOfDay> _collection;

    public WeatherForecastRepository(IMongoClient mongoClient)
    {
        _collection = mongoClient.GetDatabase("WeatherForecast").GetCollection<WeatherOfDay>("Weather");
    }

    public WeatherOfDay GetItem(Guid id)
    {
        return _collection.Find(x => x.CityName.Equals(id)).Limit(1).Single();
    }

    public void Create(WeatherOfDay weatherOfDay)
    {
        _collection.InsertOne(weatherOfDay);
    }

    public void CreateBulk(IEnumerable<WeatherOfDay> weatherOfDay)
    {
        _collection.InsertMany(weatherOfDay);
    }

    public IEnumerable<WeatherOfDay> GetWeatherOfDaysByCityName(string name, string dateTime)
    {
        var ss =  _collection.Find(x => x.CityName == name && x.Date.Equals(dateTime)).ToList();
        return ss;
    }

    public IEnumerable<string> GetCountryNames()
    {
        return _collection.Distinct(day => day.CityName, day => true).ToList();
    }
}