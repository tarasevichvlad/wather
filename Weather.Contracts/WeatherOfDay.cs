using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Weather.Contracts;

public class WeatherOfDay
{
    public Guid Id { get; set; }
    public string CityName { get; set; }
    public string Date { get; set; }
    [BsonRepresentation(BsonType.String)]
    public string TemperatureDuringTheDay { get; set; }
    [BsonRepresentation(BsonType.String)]
    public string TemperatureAtNight { get; set; }
    [BsonRepresentation(BsonType.String)]
    public string MaximumWindSpeed { get; set; }
    [BsonRepresentation(BsonType.String)]
    public string Wind { get; set; }
    [BsonRepresentation(BsonType.String)]
    public string Precipitation { get; set; }
    [BsonRepresentation(BsonType.String)]
    public string AverageDailyTemperature { get; set; }
    [BsonRepresentation(BsonType.String)]
    public string MaxPressure { get; set; }
    [BsonRepresentation(BsonType.String)]
    public string MinPressure { get; set; }
    [BsonRepresentation(BsonType.String)]
    public string Humidity { get; set; }
    [BsonRepresentation(BsonType.String)]
    public string GeomagneticActivity { get; set; }
}