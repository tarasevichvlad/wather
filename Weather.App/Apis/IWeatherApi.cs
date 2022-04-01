using Refit;
using Weather.Contracts;

namespace Weather.App.Apis;

public interface IWeatherApi
{
    [Get("/WeatherForecast?city={cityName}&date={dateTime}")]
    Task<IEnumerable<WeatherOfDay>> GetWeatherOfDaysAsync(string cityName, string dateTime);
    
    [Get("/cities")]
    Task<IEnumerable<string>> GetCountriesAsync();
}