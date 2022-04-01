namespace Weather.Persistent.Abstractions;

public interface IBulkRepository<T>
{
    void CreateBulk(IEnumerable<T> weatherOfDay);
}