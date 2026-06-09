using System;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for LeaseProposal entity operations.
/// </summary>
public interface ILeaseProposalRepository : IRepository<LeaseProposal, Guid>
{
}
