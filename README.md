# SubscriptionTracker

A minimal .NET 9 clean-architecture scaffold for a subscription-tracking API.

This README contains a short architecture summary and a living changelog describing the current wiring (DI), the Unit of Work and transaction model, and the simple migration/testing approach used by the integration tests.

## Projects

- `src/Api` — ASP.NET Core Web API (presentation / composition root)
- `src/Application` — application services, DTOs and public abstractions (interfaces)
- `src/Domain` — domain entities and domain interfaces
- `src/Infrastructure` — concrete implementations (Dapper repositories, DB wiring)
- `tests` — unit and integration tests (xUnit)

## Architecture summary (current)

- Dependency rule: inner layers (Domain, Application) declare abstractions. Outer layer (Infrastructure) implements them. Composition root (`src/Api/Program.cs`) wires implementations into DI. Services in `src/Application/Services` depend only on Application-layer interfaces (for example `IUnitOfWork`).

- Unit of Work: `IUnitOfWork` (Application layer) exposes strongly-typed repository properties (e.g., `ISubscriptionRepository Subscriptions`) and `CommitAsync` / `RollbackAsync` methods. Concrete `UnitOfWork` lives in `src/Infrastructure/Dapper` and constructs repository instances bound to a single transaction scope.

- Transaction scope: An `IDbTransactionScope` abstraction is defined in Application interfaces; Infrastructure provides `DbTransactionScope` and a factory `DbTransactionScopeFactory` that opens a connection and begins a DB transaction. Repositories accept an optional scope so they can participate in a UoW-provided transaction or run standalone.

- DI policy: The composition root registers only the application services and the factories/UoW. Individual repository implementations are not registered directly (they are created by the `UnitOfWork`), preventing accidental resolution of concrete repositories outside a unit-of-work.

## Migration & Integration testing approach

- For simple, hermetic integration tests the project uses a minimal file-based migration runner: SQL files live under `tests/Integration/migrations/*.sql` and the integration test code executes them in order before running tests.
- Integration tests run against a Postgres connection string (tests currently use a `TEST_DB` or a local constant in the test). You can switch to a Testcontainers-based approach for CI if desired.

## Notable implementation details & decisions

- Abstractions moved to Application layer: `IDbConnectionFactory`, `IDbTransactionScopeFactory`, and repository interfaces live under `src/Application/Interfaces` so Application defines the contracts and Infrastructure implements them. This enforces inward dependency direction.
- `UnitOfWork` Option B (strongly-typed repository properties) was chosen to make transactions explicit and easier to use in services.
- Services do not dispose DI-scoped `IUnitOfWork`. An `IUnitOfWorkFactory` exists for scenarios that need to create and dispose an explicit UoW instance (background jobs/tests).

## How to run (quick)

Build and run tests locally:

```powershell
dotnet build SubscriptionTracker.sln
dotnet test SubscriptionTracker.sln
```

Run API (requires configuration of a connection string named `Default` in `appsettings.json` or environment):

```powershell
cd src/Api
dotnet run
```

Integration test notes:

- Migrations: `tests/Integration/migrations/*.sql` — these are executed by integration tests prior to the tests running.
- If you prefer ephemeral DBs in CI, switch the integration tests to Testcontainers or add a docker-compose step that provides a Postgres instance and set `TEST_DB` accordingly.

## Current status & known warnings

- Tests: unit and integration tests pass locally (at time of writing). See `tests` project for specifics.
- Build may show NuGet warnings (package resolution to newer patches for Dapper/Npgsql). These are non-blocking but can be pinned in `.csproj` if deterministic restore is required.

## Next recommended cleanups (optional)

- Add a small CI check (or Roslyn analyzer) that prevents `using SubscriptionTracker.Infrastructure` inside `src/Application` to avoid accidental layering regressions.
- Consider replacing the ad-hoc migration runner with a production migration tool (DbUp or FluentMigrator) depending on needs.

See `AI_INSTRUCTIONS.md` for additional agent/automation rules used in this repo.

## Run & debug in container

These steps show a minimal way to run the API and the database in containers and debug from VS Code. The instructions assume Docker Desktop is installed and running on your machine.

1. Start Postgres with Docker (quick):

```powershell
docker run --name subtracker-postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=subscription_tracker -p 5432:5432 -d postgres:15
# wait a few seconds for DB to initialize
```

2. Run the API against the containerized Postgres:

# build image

docker build -f Dockerfile -t subscriptiontracker-api:dev src/Api

# run image, point to local Docker network Postgres (or link to db)

docker run --rm -p 5000:80 --name subscriptiontracker-api subscriptiontracker-api:dev

Set the connection string in the environment and run from solution root (PowerShell):

```powershell
$env:DefaultConnection = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=subscription_tracker"
cd src/Api
dotnet run
```

3. Using docker-compose (recommended for CI/dev): create a `docker-compose.yml` with Postgres and a service for the API (example below). Then run:

```powershell
docker-compose up --build
```

Example `docker-compose.yml` (suggestion):

```yaml
version: '3.8'
services:
	db:
		image: postgres:15
		environment:
			POSTGRES_PASSWORD: postgres
			POSTGRES_DB: subscription_tracker
		ports:
			- "5432:5432"
		volumes:
			- db-data:/var/lib/postgresql/data

	api:
		build:
			context: ./src/Api
			dockerfile: Dockerfile
		environment:
			- DefaultConnection=Host=db;Port=5432;Username=postgres;Password=postgres;Database=subscription_tracker
		ports:
			- "5000:80"
		depends_on:
			- db

volumes:
	db-data:
```

4. Debugging from VS Code:

- Add a `.devcontainer/devcontainer.json` and Dockerfile (or use the `docker-compose.yml`) to open the workspace in a container. Configure the `DefaultConnection` environment variable in the container or launch configuration.
- Use the C# extension and the default .NET Core launch profile; attach the debugger to the running `dotnet` process or start via the built-in debugger.

PowerShell note: when setting environment variables in PowerShell use `$env:VAR = 'value'`.

Cleanup: stop and remove containers when done:

```powershell
docker stop subtracker-postgres; docker rm subtracker-postgres
docker-compose down --volumes
```
