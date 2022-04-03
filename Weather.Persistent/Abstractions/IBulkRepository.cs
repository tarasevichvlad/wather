namespace Weather.Persistent.Abstractions;

public interface IBulkRepository<T>
{
    Task CreateBulkAsync(IEnumerable<T> weatherOfDay);
}