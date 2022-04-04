using PuppeteerSharp;
using Weather.Contracts;
using Weather.Persistent.Repositories;

namespace Weather.Tools.Parser;

public interface IParser
{
    /// <summary>
    /// Запускает алгоритм парсинга
    /// </summary>
    /// <returns></returns>
    Task ParseAsync();
}

public class Parser : IParser
{
    private readonly IWeatherForecastRepository _weatherForecastRepository;

    public Parser(IWeatherForecastRepository weatherForecastRepository)
    {
        _weatherForecastRepository = weatherForecastRepository;
    }

    /// <summary>
    /// Запускает алгоритм парсинга
    /// </summary>
    public async Task ParseAsync()
    {
        var browser = await LaunchBrowser();

        await using var page = await browser.NewPageAsync();

        await Parallel.ForEachAsync(GetCitiesHref(page), (href, token) =>  NavigateToCityPage(href, browser, token));

        await CloseAll(page, browser);
    }

    /// <summary>
    /// Запускает парсинг страницы с погодой конкретного города.
    /// 1) Открывает вкладку в браузере;
    /// 2) Переходит по ссылке указанной в параметре;
    /// 3) Кликает на кнопку "10 дней" на странице;
    /// 4) Запускается парисинг данных со страницы. Собрано будет информация на 10 дней;
    /// </summary>
    /// <param name="cityHref">Url страницы с погодой для конкретного города</param>
    /// <param name="browser">Browser. Необходим для открытия новой страницы, которую необходимо обработать</param>
    /// <param name="cancellationToken"></param>
    private async ValueTask NavigateToCityPage(string cityHref, Browser browser, CancellationToken cancellationToken)
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

    /// <summary>
    /// Конфигурируем и открываем браузер
    /// </summary>
    /// <returns>Запущенный браузер</returns>
    private static async Task<Browser> LaunchBrowser()
    {
        var launch = new LaunchOptions
        {
            Headless = true,
            Args = new[] { "--no-sandbox" },
        };

        return await Puppeteer.LaunchAsync(launch);
    }

    /// <summary>
    /// Собирает информацию по популярным городам России
    /// </summary>
    /// <param name="page">Вкладка браузера.</param>
    /// <returns>Ссылки на популярные города России</returns>
    private static async IAsyncEnumerable<string> GetCitiesHref(Page page)
    {
        var navigationTask = page.WaitForNavigationAsync();
        await page.GoToAsync("https://www.gismeteo.ru/");
        await navigationTask;

        var elements = await page.QuerySelectorAllAsync(Selectors.PopularCountrySelector());

        foreach (var element in elements)
        {
            yield return await element.EvaluateFunctionAsync<string>(Scripts.Href());
        }

        await page.CloseAsync();
    }

    /// <summary>
    /// Собирает информацию за каждый день.
    /// </summary>
    /// <param name="page">Вкладка браузера с которой будет собираться информация.</param>
    /// <returns>Возвращает погоду за 10 дней</returns>
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
    
    /// <summary>
    /// Собирает информацию за 1 день со страницы
    /// </summary>
    /// <param name="page">Вкладка браузера.</param>
    /// <param name="index">Индекс дня  из 10</param>
    /// <param name="cityName">Наименование города</param>
    /// <returns>Погоду за один день</returns>
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
            Date = DateTime.Now.Add(TimeSpan.FromDays(index)).ToShortDateString(),
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

    /// <summary>
    /// Вспомогательный метод для получения значения из селектора
    /// </summary>
    /// <param name="page">Вкладка браузера.</param>
    /// <param name="selector">Селектор</param>
    /// <param name="defaultValue">Значение которое будет использоваться в случае если результат получения значение селектора равен NULL</param>
    /// <returns></returns>
    private static async Task<string> GetValueBySelector(Page page, string? selector, string? defaultValue = default)
    {
        var element = await page.QuerySelectorAsync(selector);

        if (element == null)
            return defaultValue;

        var value = await element.EvaluateFunctionAsync<string>(Scripts.TextContent());

        return value;
    }

    /// <summary>
    /// Закрывает вкладку браузера и сам браузер
    /// </summary>
    /// <param name="page">Вкладка браузера.</param>
    /// <param name="browser">Браузер</param>
    private static async Task CloseAll(Page page, Browser browser)
    {
        await page.CloseAsync();
        await browser.CloseAsync();
    }
}