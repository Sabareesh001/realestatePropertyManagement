namespace propertyManagement.Repositories;

/// <summary>
/// Unit of Work interface for managing multiple repositories and database transactions.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets the User repository.
    /// </summary>
    IUserRepository Users { get; }

    /// <summary>
    /// Gets the Role repository.
    /// </summary>
    IRoleRepository Roles { get; }

    /// <summary>
    /// Saves all changes made to the database.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    /// <returns>A transaction object.</returns>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    Task CommitTransactionAsync();

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    Task RollbackTransactionAsync();
}
