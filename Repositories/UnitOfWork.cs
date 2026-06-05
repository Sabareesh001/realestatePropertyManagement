using Microsoft.EntityFrameworkCore.Storage;
using propertyManagement.Data;

namespace propertyManagement.Repositories;

/// <summary>
/// Unit of Work implementation for managing repositories and database transactions.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly PropertyManagementDbContext _context;
    private IUserRepository? _userRepository;
    private IRoleRepository? _roleRepository;
    private IDbContextTransaction? _transaction;

    /// <summary>
    /// Initializes a new instance of the UnitOfWork class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public UnitOfWork(PropertyManagementDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets the User repository instance.
    /// </summary>
    public IUserRepository Users => _userRepository ??= new UserRepository(_context);

    /// <summary>
    /// Gets the Role repository instance.
    /// </summary>
    public IRoleRepository Roles => _roleRepository ??= new RoleRepository(_context);

    /// <summary>
    /// Saves all changes made within the context to the database.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    /// <summary>
    /// Commits the current transaction and saves changes to the database.
    /// </summary>
    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            await _transaction?.CommitAsync()!;
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    /// <summary>
    /// Rolls back the current transaction and discards changes.
    /// </summary>
    public async Task RollbackTransactionAsync()
    {
        try
        {
            await _transaction?.RollbackAsync()!;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    /// <summary>
    /// Disposes the Unit of Work and its resources.
    /// </summary>
    public void Dispose()
    {
        _transaction?.Dispose();
        _context?.Dispose();
    }
}
