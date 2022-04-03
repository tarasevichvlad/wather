namespace Weather.Persistent;

public class MongoDbOptions
{
    public static string OptionName = nameof(MongoDbOptions);

    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string CollectionName { get; set; }
}