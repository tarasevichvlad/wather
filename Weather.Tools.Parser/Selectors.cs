namespace Weather.Tools.Parser;

public static class Selectors
{
    public static string Wind(int index) => $"body > section.content.wrap > div.content-column.column1 > section:nth-child(9) > div > div.widget-body.widget-columns-10 > div > div > div.widget-row.widget-row-wind-speed > div:nth-child({index + 1}) > span.wind-unit.unit.unit_wind_m_s";
    public static string MaximumWindSpeed(int index) => $"body > section.content.wrap > div.content-column.column1 > section:nth-child(9) > div > div.widget-body.widget-columns-10 > div > div > div.widget-row.widget-row-wind-gust.row-with-caption > div:nth-child({index + 2}) > span.wind-unit.unit.unit_wind_m_s";
    public static string TemperatureDuringTheDay(int index) => $"body > section.content.wrap > div.content-column.column1 > section:nth-child(2) > div > div > div > div > div.widget-row-chart.widget-row-chart-temperature > div > div > div:nth-child({index + 1}) > div.maxt > span.unit.unit_temperature_c";
    public static string TemperatureAtNight(int index) => $"body > section.content.wrap > div.content-column.column1 > section:nth-child(2) > div > div > div > div > div.widget-row-chart.widget-row-chart-temperature > div > div > div:nth-child({index + 1}) > div.mint > span.unit.unit_temperature_c";
    public static string Humidity(int index) => $"body > section.content.wrap > div.content-column.column1 > section:nth-child(11) > div > div.widget-body.widget-columns-10 > div > div > div.widget-row.widget-row-humidity > div:nth-child({index + 1})";
    public static string AverageDailyTemperature(int index) => $"body > section.content.wrap > div.content-column.column1 > section:nth-child(8) > div > div.widget-body.widget-columns-10 > div > div > div.widget-row-chart.widget-row-chart-temperature-avg > div > div > div:nth-child({index + 1}) > span.unit.unit_temperature_c";
    public static string MaxPressure(int index) => $"body > section.content.wrap > div.content-column.column1 > section:nth-child(10) > div > div.widget-body.widget-columns-10 > div > div > div.widget-row-chart.widget-row-chart-pressure > div > div > div:nth-child({index + 1}) > div.maxt > span.unit.unit_pressure_mm_hg_atm";
    public static string MinPressure(int index) => $"body > section.content.wrap > div.content-column.column1 > section:nth-child(10) > div > div.widget-body.widget-columns-10 > div > div > div.widget-row-chart.widget-row-chart-pressure > div > div > div:nth-child({index + 1}) > div.mint > span.unit.unit_pressure_mm_hg_atm";
    public static string Precipitation(int index) => $"body > section.content.wrap > div.content-column.column1 > section:nth-child(2) > div > div > div > div > div.widget-row.widget-row-precipitation-bars.row-with-caption > div:nth-child({index + 2}) > div";
    public static string GeomagneticActivity(int index) => $"body > section.content.wrap > div.content-column.column1 > section:nth-child(13) > div > div.widget-body.widget-columns-10 > div > div > div.widget-row.widget-row-geomagnetic > div:nth-child({index + 1}) > div";
    public static string CityName() => "body > header > div.header-subnav > div.header-container.wrap > div > div.search.js-search > div.search-form.js-search-form > div > input";
    public static string TenDaySelector() => "body > header > div.header-subnav > div.header-container.wrap > div > div.subnav > div > a:nth-child(5)";
    public static string PopularCountrySelector() => "div.cities-popular > div.list > div.list-item > a";
        
}