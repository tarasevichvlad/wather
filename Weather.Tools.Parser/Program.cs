using MongoDB.Driver;
using PuppeteerSharp;
using PuppeteerSharp.Input;
using Weather.Contracts;
using Weather.Persistent.Repositories;
using Weather.Tools.Parser;

var mongoClient = new MongoClient("mongodb://root:example@localhost:27017");

var repository = new WeatherForecastRepository(mongoClient);

await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);

var launch = new LaunchOptions
{
    Headless = false
};

var browser = await Puppeteer.LaunchAsync(launch);

await using var page = await browser.NewPageAsync();
await page.GoToAsync("https://www.gismeteo.ru/");
var elements = await page.QuerySelectorAllAsync(Selectors.PopularCountrySelector());
var hrefs = new List<string>();
foreach (var element in elements)
{
    var href = await element.EvaluateFunctionAsync<string>(Scripts.Href());
    hrefs.Add(href);
}

foreach (var href in hrefs)
{
    await page.GoToAsync(href);
    await page.ClickAsync(Selectors.TenDaySelector());
    await Task.Delay(1000);

    var cityNameElement = await page.QuerySelectorAsync(Selectors.CityName());
    var cityName = await cityNameElement.EvaluateFunctionAsync<string>(Scripts.Placeholder());
    
    var collection = Enumerable.Range(0, 10);
    var cityCollection = new List<WeatherOfDay>();
    
    foreach (var index in collection)
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

        cityCollection.Add(weatherOfDay);
    }
    
    repository.CreateBulk(cityCollection);
}

await page.CloseAsync();
await browser.CloseAsync();

async Task<string> GetValueBySelector(Page page, string? selector, string? defaultValue = default)
{
    var element = await page.QuerySelectorAsync(selector);

    if (element == null)
        return defaultValue;

    var value = await element.EvaluateFunctionAsync<string>(Scripts.TextContent());

    return value;
};