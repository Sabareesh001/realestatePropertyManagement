namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object representing server-side aggregated finance figures for the admin dashboard.
/// </summary>
public class AdminFinanceSummaryDto
{
    /// <summary>
    /// Gets or sets the total platform fees earned by the company across all completed payments.
    /// </summary>
    public decimal CompanyRevenue { get; set; }

    /// <summary>
    /// Gets or sets the gross volume of all completed payments processed through the platform.
    /// </summary>
    public decimal GrossVolume { get; set; }

    /// <summary>
    /// Gets or sets the total amount of payments that are still pending.
    /// </summary>
    public decimal PendingAmount { get; set; }

    /// <summary>
    /// Gets or sets the total number of payments.
    /// </summary>
    public int PaymentCount { get; set; }

    /// <summary>
    /// Gets or sets the number of completed payments.
    /// </summary>
    public int CompletedCount { get; set; }

    /// <summary>
    /// Gets or sets the number of failed payments.
    /// </summary>
    public int FailedCount { get; set; }

    /// <summary>
    /// Gets or sets the number of refunded payments.
    /// </summary>
    public int RefundedCount { get; set; }
}
