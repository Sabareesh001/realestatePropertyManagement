using System;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for Document entity operations.
/// </summary>
public interface IDocumentRepository : IRepository<Document, Guid>
{
}
