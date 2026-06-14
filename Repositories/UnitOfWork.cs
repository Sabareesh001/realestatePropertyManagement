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
    private IUserVerificationRepository? _userVerificationRepository;
    private IPropertyRepository? _propertyRepository;
    private ICityRepository? _cityRepository;
    private ILeaseProposalRepository? _leaseProposalRepository;
    private ILeaseRepository? _leaseRepository;
    private IBankAccountRepository? _bankAccountRepository;
    private IPropertyImageRepository? _propertyImageRepository;
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
    /// Gets the UserVerification repository instance.
    /// </summary>
    public IUserVerificationRepository UserVerifications => _userVerificationRepository ??= new UserVerificationRepository(_context);

    /// <summary>
    /// Gets the Property repository instance.
    /// </summary>
    public IPropertyRepository Properties => _propertyRepository ??= new PropertyRepository(_context);

    /// <summary>
    /// Gets the City repository instance.
    /// </summary>
    public ICityRepository Cities => _cityRepository ??= new CityRepository(_context);

    /// <summary>
    /// Gets the LeaseProposal repository instance.
    /// </summary>
    public ILeaseProposalRepository LeaseProposals => _leaseProposalRepository ??= new LeaseProposalRepository(_context);

    /// <summary>
    /// Gets the Lease repository instance.
    /// </summary>
    public ILeaseRepository Leases => _leaseRepository ??= new LeaseRepository(_context);

    /// <summary>
    /// Gets the BankAccount repository instance.
    /// </summary>
    public IBankAccountRepository BankAccounts => _bankAccountRepository ??= new BankAccountRepository(_context);

    /// <summary>
    /// Gets the PropertyImage repository instance.
    /// </summary>
    public IPropertyImageRepository PropertyImages => _propertyImageRepository ??= new PropertyImageRepository(_context);

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
