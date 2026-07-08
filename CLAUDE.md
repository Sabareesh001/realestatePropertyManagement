# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build

# Run (development)
dotnet run

# Run tests (all)
dotnet test propertyManagement.Tests/

# Run a single test class
dotnet test propertyManagement.Tests/ --filter "FullyQualifiedName~BankAccountServiceTests"

# EF Core migrations
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

## Architecture

ASP.NET Core (.NET 10) Web API targeting PostgreSQL via EF Core + Npgsql. The test project is `propertyManagement.Tests/` using NUnit + Moq.

### Layer Flow

```
Controller → Service → IUnitOfWork → Repository → DbContext (EF Core)
```

- **Controllers** (`Controllers/`) — thin; delegate all logic to services injected via constructor DI.
- **Services** (`Services/`) — business logic; accept `IUnitOfWork` (not individual repositories) so they transact across entities cleanly.
- **Repositories** (`Repositories/`) — per-entity data access behind interfaces. Never instantiated directly — always accessed through `IUnitOfWork`.
- **UnitOfWork** (`Repositories/UnitOfWork.cs`) — single entry point to all repositories; wraps `PropertyManagementDbContext`. Exposes `BeginTransactionAsync / CommitTransactionAsync / RollbackTransactionAsync` for multi-step writes.
- **DTOs** (`DTOs/`) — separate request/response types; never expose EF entities directly.
- **Validators** (`Validators/`) — FluentValidation validators, one per DTO. Auto-registered from assembly and applied globally via `ValidationFilter` (`Filters/ValidationFilter.cs`), which runs before every action and returns 400 on failure — no manual validation needed in controllers.
- **GlobalExceptionHandler** (`Middlewares/GlobalExceptionHandler.cs`) — maps exception types to HTTP status codes (`InvalidOperationException` → 400, `KeyNotFoundException` → 404, `UnauthorizedAccessException` → 403, `StripeException` → 400/502). Let exceptions propagate; avoid try-catch in controllers and services.

### Authentication

JWT Bearer tokens validated from the `Authorization` header **or** a `jwt_token` HTTP cookie. Config lives under `appsettings.json → Jwt`. All protected routes use standard `[Authorize]` attributes.

### Stripe Integration

- `IStripeGateway` / `StripeGateway` — thin wrapper around Stripe SDK service classes (singleton).
- `IStripeConnectService` / `StripeConnectService` — business logic for Stripe Connect onboarding and payouts.
- `StripeController` — webhook and onboarding endpoints.
- Config: `appsettings.json → Stripe` (SecretKey, WebhookSecret, PlatformFeePercent, etc.).

### Key Conventions (from AGENTS.md)

- **One class/interface/DTO per file**, file name matches type name exactly.
- **XML `///` summary comments** on every public class, method, property, and interface — include `<param>`, `<returns>`, `<exception>` where applicable.
- **Before implementing a feature**, outline the plan (files to create/modify, endpoints, DTOs, repository methods, DI registrations) and wait for approval.
- All I/O is async/await end-to-end.
- Register every new service/repository in `Program.cs`.

### Configuration

`appsettings.json` holds DB connection string, JWT settings, Stripe keys, Serilog config, and `FrontendUrl` (CORS origin, default `http://localhost:4200`). Sensitive Stripe keys should be set via user secrets or environment variables rather than committed to `appsettings.json`.
