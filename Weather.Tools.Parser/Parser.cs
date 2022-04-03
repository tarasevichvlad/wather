using PuppeteerSharp;
using Weather.Contracts;
using Weather.Persistent.Repositories;

namespace Weather.Tools.Parser;

public interface IParser
{
    Task ParseAsync();
}

public class Parser : IParser
{
    private readonly IWeatherForecastRepository _weatherForecastRepository;

    public Parser(IWeatherForecastRepository weatherForecastRepository)
    {
        _weatherForecastRepository = weatherForecastRepository;
    }

    public async Task ParseAsync()
    {
        var browser = await LaunchBrowser();

        await using var page = await browser.NewPageAsync();

        await Parallel.ForEachAsync(GetCitiesHref(page), (href, token) =>  Body(href, browser, token));

        await CloseAll(page, browser);
    }

    private async ValueTask Body(string cityHref, Browser browser, CancellationToken cancellationToken)
    {
        await using var page = await browser.NewPageAsync();
        var navigationTask = page.WaitForNavigationAsync();
        await page.GoToAsync(cityHref);
        await page.ClickAsync(Selectors.TenDaySelector());
        await navigationTask;

        await foreach (var weatherOfDay in GetWeatherOfDays(page).WithCancellation(cancellationToken))
        {
            await _weatherForecastRepository.CreateAsync(weatherOfDay);
        }

        await page.CloseAsync();
    }

    private static async Task<Browser> LaunchBrowser()
    {
        var launch = new LaunchOptions
        {
            Headless = true,
            Args = new[] { "--no-sandbox" },
        };

        return await Puppeteer.LaunchAsync(launch);
    }

    private static async IAsyncEnumerable<string> GetCitiesHref(Page page)
    {
        await page.GoToAsync("https://www.gismeteo.ru/");

        var elements = await page.QuerySelectorAllAsync(Selectors.PopularCountrySelector());

        foreach (var element in elements)
        {
            yield return await element.EvaluateFunctionAsync<string>(Scripts.Href());
        }
        
        await page.CloseAsync();
    }

    private static async IAsyncEnumerable<WeatherOfDay> GetWeatherOfDays(Page page)
    {
        var cityNameElement = await page.WaitForSelectorAsync(Selectors.CityName());
        var cityName = await cityNameElement.EvaluateFunctionAsync<string>(Scripts.Placeholder());
    
        var collection = Enumerable.Range(0, 10);

        foreach (var index in collection)
        {
            yield return await GetWeatherOfDay(page, index, cityName);
        }
    }
    
    private static async Task<WeatherOfDay> GetWeatherOfDay(Page page, int index, string cityName)
    {
        var wind = await GetValueBySelector(page, Selectors.Wind(index));
        var maximumWindSpeed = await GetValueBySelector(page, Selectors.MaximumWindSpeed(index));
        var temperatureDuringTheDay = await GetValueBySelector(page, Selectors.TemperatureDuringTheDay(index));
        var temperatureAtNight = await GetValueBySelector(page, Selectors.TemperatureAtNight(index));
        var humidity = await GetValueBySelector(page, Selectors.Humidity(index));
        var averageDailyTemperature = await GetValueBySelector(page, Selectors.AverageDailyTemperature(index));
        var maxPressure = await GetValueBySelector(page, Selectors.MaxPressure(index));
        var minPressure = await GetValueBySelector(page, Selectors.MinPressure(index), maxPressure);
        var precipitation = await GetValueBySelector(page, Selectors.Precipitation(index));
        var geomagneticActivity = await GetValueBySelector(page, Selectors.GeomagneticActivity(index));

        var weatherOfDay = new WeatherOfDay
        {
            Id = Guid.NewGuid(),
            Date = DateTime.Now.Add(TimeSpan.FromDays(1 + index)).ToShortDateString(),
            CityName = cityName,
            Wind = wind,
            MaximumWindSpeed = maximumWindSpeed,
            TemperatureDuringTheDay = temperatureDuringTheDay,
            TemperatureAtNight = temperatureAtNight,
            Humidity = humidity,
            AverageDailyTemperature = averageDailyTemperature,
            MaxPressure = maxPressure,
            MinPressure = minPressure,
            Precipitation = precipitation,
            GeomagneticActivity = geomagneticActivity
        };

        return weatherOfDay;
    }

    private static async Task<string> GetValueBySelector(Page page, string? selector, string? defaultValue = default)
    {
        var element = await page.QuerySelectorAsync(selector);

        if (element == null)
            return defaultValue;

        var value = await element.EvaluateFunctionAsync<string>(Scripts.TextContent());

        return value;
    }

    private static async Task CloseAll(Page page, Browser browser)
    {
        await page.CloseAsync();
        await browser.CloseAsync();
    }
}