---
description: "Use when: working on the Property Management ASP.NET Core project with controllers, DTOs, and repositories"
applyTo: "**/*.cs"
---

# Property Management Project - Copilot Instructions

## Project Overview
This is an ASP.NET Core project for property management. All code should follow the guidelines outlined below to maintain consistency, maintainability, and best practices.

## Key Coding Guidelines

### 1. API Endpoints - Always Use Controllers
- All API endpoints must be implemented in dedicated controller classes
- Controllers should be placed in the `Controllers/` directory
- Use attribute-based routing (`[Route(...)]`, `[HttpGet]`, etc.)
- Keep controllers focused and single-responsibility
- Use naming convention: `[EntityName]Controller.cs`

### 2. Dependency Injection
- Always use constructor-based dependency injection
- Register dependencies in `Program.cs` using the service container
- Never instantiate services directly with `new` keyword
- Use interfaces for all injected dependencies
- Leverage ASP.NET Core's built-in DI container

### 3. Repository Pattern
- Implement repository interfaces for data access
- Create separate repository files for each entity
- Repository interfaces should define contract for data operations (CRUD)
- Repository implementations should handle all database interactions
- Use generic repositories where applicable to reduce code duplication
- Example structure:
  - `Repositories/IRepository.cs` (generic interface)
  - `Repositories/IEntityRepository.cs` (specific interface)
  - `Repositories/EntityRepository.cs` (implementation)

### 4. XML Summary Comments
- Every public class, method, property, and interface must have XML documentation comments
- Use `///` for XML comments
- Include meaningful descriptions of purpose and functionality
- Document parameters with `<param>` tags
- Document return values with `<returns>` tags
- Document exceptions with `<exception>` tags where applicable
- Example:
  ```csharp
  /// <summary>
  /// Retrieves a property by its unique identifier.
  /// </summary>
  /// <param name="id">The unique identifier of the property.</param>
  /// <returns>The property entity if found; otherwise null.</returns>
  /// <exception cref="ArgumentException">Thrown when id is invalid.</exception>
  public Task<Property> GetPropertyByIdAsync(int id)
  ```

### 5. Best Practices
- Follow SOLID principles
- Use async/await for all I/O operations
- Implement proper error handling with custom exceptions
- Use middleware for cross-cutting concerns (logging, exception handling)
- Validate input in controllers before processing
- Return appropriate HTTP status codes
- Use nullable reference types (`#nullable enable`)
- Follow C# naming conventions (PascalCase for classes, camelCase for variables)
- Keep methods focused and small (single responsibility)

### 6. DTOs (Data Transfer Objects)
- Use DTOs for API request/response payloads
- DTOs should be separate from domain entities
- Create DTOs when:
  - Exposing entities via APIs
  - Accepting complex input from clients
  - Need to hide internal implementation details
  - Transforming data between layers
- Place DTOs in `DTOs/` directory
- Use descriptive names: `CreatePropertyDto.cs`, `PropertyResponseDto.cs`, etc.

### 7. File Organization - One Class Per File
- Each class, interface, DTO, and enum should have its own file
- File name should match the class name exactly
- Organize files in appropriate directories:
  - `Controllers/` - API controllers
  - `DTOs/` - Data transfer objects
  - `Models/` or `Entities/` - Domain entities
  - `Repositories/` - Repository interfaces and implementations
  - `Services/` - Business logic services
  - `Exceptions/` - Custom exception classes
  - `Middlewares/` - Custom middleware
  - `Interfaces/` - Service and other interfaces (if not co-located)

### 8. Before Implementation - Show Your Plan First
When asked to implement a feature:
1. First, analyze the requirements
2. Outline the plan including:
   - What files need to be created or modified
   - Controller endpoints to be added
   - DTOs needed
   - Repository methods required
   - Any dependencies to be injected
   - Key implementation details
3. Wait for approval before starting implementation
4. Execute the plan step-by-step

### 9. Exception Handling - Use Global Exception Handler Middleware
- **Avoid manual try-catch blocks** in controllers and services as much as possible
- Utilize the `GlobalExceptionHandler` middleware located in `Middlewares/GlobalExceptionHandler.cs`
- The middleware automatically handles exceptions and returns appropriate HTTP responses
- Only use try-catch when you need specific handling before the exception propagates to middleware
- Let exceptions bubble up to the middleware for centralized, consistent error handling
- This ensures:
  - Consistent error response format across all endpoints
  - Centralized logging of exceptions
  - Reduced code duplication
  - Better maintainability and error tracking

## Example Structure for New Feature

```
Feature: Create Property Management Endpoint

Plan:
1. Create PropertyDto.cs (DTOs directory)
   - CreatePropertyDto
   - UpdatePropertyDto
   - PropertyResponseDto

2. Create IPropertyRepository.cs and PropertyRepository.cs (Repositories directory)
   - GetPropertyByIdAsync
   - GetAllPropertiesAsync
   - CreatePropertyAsync
   - UpdatePropertyAsync
   - DeletePropertyAsync

3. Create PropertyController.cs (Controllers directory)
   - GET /api/properties
   - GET /api/properties/{id}
   - POST /api/properties
   - PUT /api/properties/{id}
   - DELETE /api/properties/{id}

4. Update Program.cs
   - Register IPropertyRepository

5. Add XML documentation to all public members
```

## Code Example Template

```csharp
namespace PropertyManagement.Controllers;

using PropertyManagement.DTOs;
using PropertyManagement.Repositories;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing property-related endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PropertyController : ControllerBase
{
    private readonly IPropertyRepository _propertyRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyController"/> class.
    /// </summary>
    /// <param name="propertyRepository">The property repository for data access.</param>
    public PropertyController(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository ?? throw new ArgumentNullException(nameof(propertyRepository));
    }

    /// <summary>
    /// Retrieves a property by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the property.</param>
    /// <returns>The property details if found.</returns>
    /// <response code="200">Returns the property.</response>
    /// <response code="404">Property not found.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<PropertyResponseDto>> GetPropertyById(int id)
    {
        var property = await _propertyRepository.GetPropertyByIdAsync(id);
        if (property == null)
        {
            return NotFound();
        }

        return Ok(property);
    }
}
```

## Summary
- **Always** use controllers for endpoints
- **Always** use dependency injection
- **Always** implement repository pattern
- **Always** add XML summary comments
- **Always** show plan before implementing
- Use DTOs appropriately
- One file per class/interface/DTO
- Follow SOLID and .NET best practices
