namespace Weather.Persistent.Abstractions;

public interface IRepository<T>
{
    T GetItem(Guid id);
    void Create(T weatherOfDay);
}

