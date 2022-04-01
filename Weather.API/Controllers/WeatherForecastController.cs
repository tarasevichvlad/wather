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
    public IEnumerable<WeatherOfDay> Get([FromQuery(Name = "city")] string city, [FromQuery(Name = "date")] DateTime date)
    {
        return _weatherForecastRepository.GetWeatherOfDaysByCityName(city, date.ToShortDateString());
    }

    [HttpGet("/cities")]
    public IEnumerable<string> Get()
    {
        return _weatherForecastRepository.GetCountryNames();
    }
}