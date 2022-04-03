using Microsoft.AspNetCore.Mvc;
using Weather.Contracts;
using Weather.Persistent.Repositories;

namespace Weather.API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherForecastRepository _weatherForecastRepository;

    public WeatherForecastController(IWeatherForecastRepository weatherForecastRepository)
    {
        _weatherForecastRepository = weatherForecastRepository;
    }

    [HttpGet]
    public async Task<IEnumerable<WeatherOfDay>> GetWeatherOfDays([FromQuery(Name = "city")] string city, [FromQuery(Name = "date")] DateTime date)
    {
        return await _weatherForecastRepository.GetWeatherOfDaysByCityNameAsync(city, date.ToShortDateString());
    }

    [HttpGet("/cities")]
    public async Task<IEnumerable<string>> GetCities()
    {
        return await _weatherForecastRepository.GetCountryNamesAsync();
    }
}