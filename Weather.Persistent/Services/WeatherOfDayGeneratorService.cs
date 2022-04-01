using Weather.Contracts;

namespace Weather.Persistent.Services;

public interface IWeatherOfDayGeneratorService
{
    IEnumerable<WeatherOfDay> GenerateWeatherOfDays(int count);
}

public class WeatherOfDayGeneratorService : IWeatherOfDayGeneratorService
{
    private readonly Random _random = new();

    public IEnumerable<WeatherOfDay> GenerateWeatherOfDays(int count)
    {
        var cities = new[] { "Минск", "Гродно", "Полоцк", "Слуцк" };

        return Enumerable.Range(0, count)
            .Select(_ => new WeatherOfDay
            {
                Id = Guid.NewGuid(),
                CityName = cities[_random.Next(cities.Length - 1)],
                Date = (new DateTime(2022, _random.Next(1, 12), _random.Next(1, 31))).ToShortDateString(),
                TemperatureDuringTheDay = _random.Next(1, 31).ToString(),
                TemperatureAtNight = _random.Next(1, 31).ToString(),
                MaximumWindSpeed = _random.Next(1, 31).ToString(),
                Wind = _random.Next(1, 31).ToString(),
                Precipitation = _random.Next(1, 31).ToString(),
                AverageDailyTemperature = _random.Next(1, 31).ToString(),
                MaxPressure = _random.Next(1, 31).ToString(),
                MinPressure = _random.Next(1, 31).ToString(),
                Humidity = _random.Next(1, 31).ToString(),
                GeomagneticActivity = _random.Next(1, 31).ToString(),
            });
    }
}