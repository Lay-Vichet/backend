using Dapper;
using Npgsql;
using Xunit;
using SubscriptionTracker.Infrastructure.Dapper;
using SubscriptionTracker.Application.Interfaces;
using SubscriptionTracker.Infrastructure.Repositories;
using SubscriptionTracker.Application.DTOs;
using System.IO;

namespace SubscriptionTracker.IntegrationTests;

public class TransactionIntegrationTests
{
    private const string testDbConnectionString = "Host=localhost;Port=5432;Username=postgres;Password=123456;Database=TEST_DB";

    [Fact]
    public async Task Commit_persists_changes()
    {
        var connString = testDbConnectionString;
        if (string.IsNullOrWhiteSpace(connString))
        {
            return; // skip if no TEST_DB provided
        }

        // apply migrations by executing SQL files from tests/Integration/migrations
        var migrationsPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "tests", "Integration", "migrations"));
        if (Directory.Exists(migrationsPath))
        {
            var files = Directory.GetFiles(migrationsPath, "*.sql").OrderBy(x => x);
            await using var connForMigrations = new NpgsqlConnection(connString);
            await connForMigrations.OpenAsync();
            foreach (var file in files)
            {
                var sql = await File.ReadAllTextAsync(file);
                await connForMigrations.ExecuteAsync(sql);
            }
        }

        await using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync();

        var connectionFactory = new TestConnectionFactory(connString);
        var txFactory = new DbTransactionScopeFactory(connectionFactory);

        // use UnitOfWork to create a subscription and commit
        await using var uow = new DbUnitOfWorkAdapter(txFactory, connectionFactory); // adapter we'll implement below in test project

        var id = Guid.NewGuid();
        await uow.Subscriptions.AddAsync(new SubscriptionDto { Id = id, Name = "committed", MonthlyCost = 1.23M, StartDate = System.DateTime.UtcNow });
        await uow.CommitAsync();
        await uow.DisposeAsync();

        var count = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM subscriptions WHERE id = @Id", new { Id = id });
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task Rollback_discards_changes()
    {
        var connString = testDbConnectionString;
        if (string.IsNullOrWhiteSpace(connString))
        {
            return; // skip if no TEST_DB provided
        }

        // apply migrations from tests/migrations using DbUp
        // apply migrations by executing SQL files from tests/Integration/migrations
        var migrationsPath2 = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "tests", "Integration", "migrations"));
        if (Directory.Exists(migrationsPath2))
        {
            var files2 = Directory.GetFiles(migrationsPath2, "*.sql").OrderBy(x => x);
            await using var connForMigrations2 = new NpgsqlConnection(connString);
            await connForMigrations2.OpenAsync();
            foreach (var file in files2)
            {
                var sql = await File.ReadAllTextAsync(file);
                await connForMigrations2.ExecuteAsync(sql);
            }
        }

        await using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync();

        var connectionFactory = new TestConnectionFactory(connString);
        var txFactory = new DbTransactionScopeFactory(connectionFactory);

        await using var uow = new DbUnitOfWorkAdapter(txFactory, connectionFactory);

        var id = Guid.NewGuid();
        await uow.Subscriptions.AddAsync(new SubscriptionDto { Id = id, Name = "rolledback", MonthlyCost = 4.56M, StartDate = System.DateTime.UtcNow });
        await uow.RollbackAsync();
        await uow.DisposeAsync();

        var count = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM subscriptions WHERE id = @Id", new { Id = id });
        Assert.Equal(0, count);
    }
}

// --- Test helpers implemented in-test-project ---
public class TestConnectionFactory : IDbConnectionFactory
{
    private readonly string _conn;
    public TestConnectionFactory(string connectionString) => _conn = connectionString;
    public System.Data.IDbConnection CreateConnection() => new NpgsqlConnection(_conn);
}

// The production UnitOfWork type is in Infrastructure.Dapper.UnitOfWork and implements IUnitOfWork.
// However, it's internal to production assembly details; to avoid referencing internal constructors we create a small adapter that constructs the real repositories bound to a scope.
public class DbUnitOfWorkAdapter : IUnitOfWork
{
    private readonly IDbTransactionScope _scope;
    public ISubscriptionRepository Subscriptions { get; }
    // Other repos omitted for brevity - tests only need Subscriptions. Provide no-op implementations.
    public ICategoryRepository Categories => throw new NotImplementedException();
    public IHouseholdRepository Households => throw new NotImplementedException();
    public IHouseholdMemberRepository HouseholdMembers => throw new NotImplementedException();
    public IPaymentMethodRepository PaymentMethods => throw new NotImplementedException();
    public ISharedSubscriptionRepository SharedSubscriptions => throw new NotImplementedException();
    public ISubscriptionCategoryRepository SubscriptionCategories => throw new NotImplementedException();
    public ISubscriptionPaymentRepository SubscriptionPayments => throw new NotImplementedException();
    public ISubscriptionRatingRepository SubscriptionRatings => throw new NotImplementedException();
    public ISubscriptionUsageRepository SubscriptionUsages => throw new NotImplementedException();
    public IUserRepository Users => throw new NotImplementedException();

    public DbUnitOfWorkAdapter(IDbTransactionScopeFactory txFactory, IDbConnectionFactory connectionFactory)
    {
        _scope = txFactory.BeginTransaction();
        Subscriptions = new DapperSubscriptionRepository(connectionFactory, _scope);
    }

    public Task CommitAsync()
    {
        _scope.Commit();
        return Task.CompletedTask;
    }

    public Task RollbackAsync()
    {
        _scope.Rollback();
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync() => _scope.DisposeAsync();
}
