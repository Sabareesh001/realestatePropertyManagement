namespace propertyManagement.Models;

/// <summary>
/// Catalog of notification event types used across the lease/lease-proposal approval workflow.
/// </summary>
public static class NotificationType
{
    /// <summary>A tenant submitted a lease proposal; the property owner is notified.</summary>
    public const int ProposalSubmitted = 1;

    /// <summary>The owner approved a lease proposal; the tenant is notified.</summary>
    public const int ProposalApproved = 2;

    /// <summary>The owner rejected a lease proposal; the tenant is notified.</summary>
    public const int ProposalRejected = 3;

    /// <summary>The owner submitted a lease draft for admin approval; all admins are notified.</summary>
    public const int LeaseSubmittedForApproval = 4;

    /// <summary>An admin approved a lease template; the owner and tenant are notified.</summary>
    public const int LeaseTemplateApproved = 5;

    /// <summary>An admin rejected a lease template; the owner and tenant are notified.</summary>
    public const int LeaseTemplateRejected = 6;

    /// <summary>The tenant signed the lease agreement; the owner and all admins are notified.</summary>
    public const int LeaseSignedSubmitted = 7;

    /// <summary>An admin approved the tenant-signed lease; the owner and tenant are notified.</summary>
    public const int LeaseSignedApproved = 8;

    /// <summary>An admin rejected the tenant-signed lease; the owner and tenant are notified.</summary>
    public const int LeaseSignedRejected = 9;
}
