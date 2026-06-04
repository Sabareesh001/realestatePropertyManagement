namespace propertyManagement.Repositories
{
    public interface IRepository<T, TId> where T : class
    {
        Task<T> CreateAsync(T entity);
        Task<T?> GetByIdAsync(TId id);
        Task<IEnumerable<T>> GetAllAsync();
        Task UpdateAsync(T entity);
        Task DeleteAsync(TId id);
    }

    // Backward compatibility - default to int ID
    public interface IRepository<T> : IRepository<T, int> where T : class
    {
    }
}