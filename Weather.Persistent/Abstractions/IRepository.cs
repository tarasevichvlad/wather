namespace Weather.Persistent.Abstractions;

public interface IRepository<T>
{
    Task<T> GetItemAsync(Guid id);
    Task CreateOrUpdateAsync(T weatherOfDay);
}

