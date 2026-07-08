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
    /// Gets the UserVerification repository.
    /// </summary>
    IUserVerificationRepository UserVerifications { get; }

    /// <summary>
    /// Gets the Property repository.
    /// </summary>
    IPropertyRepository Properties { get; }

    /// <summary>
    /// Gets the City repository.
    /// </summary>
    ICityRepository Cities { get; }

    /// <summary>
    /// Gets the LeaseProposal repository.
    /// </summary>
    ILeaseProposalRepository LeaseProposals { get; }

    /// <summary>
    /// Gets the Lease repository.
    /// </summary>
    ILeaseRepository Leases { get; }

    /// <summary>
    /// Gets the Document repository.
    /// </summary>
    IDocumentRepository Documents { get; }

    /// <summary>
    /// Gets the BankAccount repository.
    /// </summary>
    IBankAccountRepository BankAccounts { get; }


    /// <summary>
    /// Gets the PropertyImage repository.
    /// </summary>
    IPropertyImageRepository PropertyImages { get; }

    /// <summary>
    /// Gets the Charge repository.
    /// </summary>
    IChargeRepository Charges { get; }

    /// <summary>
    /// Gets the Payment repository.
    /// </summary>
    IPaymentRepository Payments { get; }

    /// <summary>
    /// Gets the Complaint repository.
    /// </summary>
    IComplaintRepository Complaints { get; }

    /// <summary>
    /// Gets the ComplaintComment repository.
    /// </summary>
    IComplaintCommentRepository ComplaintComments { get; }

    /// <summary>
    /// Gets the Notification repository.
    /// </summary>
    INotificationRepository Notifications { get; }

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
