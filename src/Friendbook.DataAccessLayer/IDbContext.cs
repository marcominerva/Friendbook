namespace Friendbook.DataAccessLayer;

public interface IDbContext
{
    IQueryable<T> GetData<T>(bool trackingChanges = false) where T : class;

    public IQueryable<T> GetData<T>(string query, params object[] parameters) where T : class
        => GetData<T>(query, false, parameters);

    public IQueryable<T> GetData<T>(string query, bool trackingChanges = false, params object[] parameters) where T : class;

    void Insert<T>(T entity) where T : class;

    void Delete<T>(T entity) where T : class;

    Task<int> SaveAsync();

    Task MigrateAsync();
}