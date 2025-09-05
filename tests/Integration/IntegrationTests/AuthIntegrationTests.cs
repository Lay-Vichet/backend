using Xunit;
using Dapper;
using Npgsql;
using SubscriptionTracker.Infrastructure.Dapper;
using SubscriptionTracker.Application.Interfaces;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Infrastructure.Auth;
using Microsoft.Extensions.Configuration;

namespace SubscriptionTracker.IntegrationTests;

public class AuthIntegrationTests
{
    private const string testDbConnectionString = "Host=localhost;Port=5432;Username=postgres;Password=123456;Database=TEST_DB";

    private IConfiguration BuildConfig()
    {
        var configBuilder = new ConfigurationBuilder();
        // Use a 32-character key for HS256 in tests
        configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string?>("JWT:Key", "test-secret-which-is-not-secureX"), new KeyValuePair<string, string?>("JWT:ExpiresMinutes", "60") });
        return configBuilder.Build();
    }

    [Fact]
    public async Task Register_and_login_happy_path()
    {
        var connString = testDbConnectionString;
        if (string.IsNullOrWhiteSpace(connString)) return;

        // apply migrations - search upward from the test binary location for tests/Integration/migrations
        string? migrationsPath = null;
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            var candidate = Path.Combine(current.FullName, "tests", "Integration", "migrations");
            if (Directory.Exists(candidate))
            {
                migrationsPath = candidate;
                break;
            }
            current = current.Parent;
        }
        Console.WriteLine($"[TEST] Resolved migrationsPath: {migrationsPath}");
        if (!string.IsNullOrEmpty(migrationsPath))
        {
            await using var connForMigrations = new NpgsqlConnection(connString);
            await connForMigrations.OpenAsync();
            var files = Directory.GetFiles(migrationsPath, "*.sql").OrderBy(x => x);
            foreach (var file in files)
            {
                Console.WriteLine($"[TEST] Applying migration: {file}");
                var sql = await File.ReadAllTextAsync(file);
                await connForMigrations.ExecuteAsync(sql);
            }
        }

        var connectionFactory = new TestConnectionFactory(connString);
        var txFactory = new DbTransactionScopeFactory(connectionFactory);

        // simple factory for tests
        IUnitOfWorkFactory uowFactory = new TestUnitOfWorkFactory(txFactory, connectionFactory);
        var authService = new AuthService(uowFactory, BuildConfig());

        var email = $"test{System.Guid.NewGuid():N}@example.com";
        var register = new RegisterRequest { Email = email, Password = "Password123!" };
        var resp = await authService.RegisterAsync(register);
        Assert.False(string.IsNullOrEmpty(resp.Token));

        var login = new LoginRequest { Email = email, Password = "Password123!" };
        var loginResp = await authService.LoginAsync(login);
        Assert.False(string.IsNullOrEmpty(loginResp.Token));
    }

    [Fact]
    public async Task Register_duplicate_email_returns_conflict()
    {
        var connString = testDbConnectionString;
        if (string.IsNullOrWhiteSpace(connString)) return;

        // apply migrations
        var migrationsPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "tests", "Integration", "migrations"));
        if (Directory.Exists(migrationsPath))
        {
            await using var connForMigrations = new NpgsqlConnection(connString);
            await connForMigrations.OpenAsync();
            var files = Directory.GetFiles(migrationsPath, "*.sql").OrderBy(x => x);
            foreach (var file in files)
            {
                var sql = await File.ReadAllTextAsync(file);
                await connForMigrations.ExecuteAsync(sql);
            }
        }

        var connectionFactory = new TestConnectionFactory(connString);
        var txFactory = new DbTransactionScopeFactory(connectionFactory);
        IUnitOfWorkFactory uowFactory = new TestUnitOfWorkFactory(txFactory, connectionFactory);
        var authService = new AuthService(uowFactory, BuildConfig());

        var email = $"dup{System.Guid.NewGuid():N}@example.com";
        var register = new RegisterRequest { Email = email, Password = "Password123!" };
        var resp = await authService.RegisterAsync(register);
        Assert.False(string.IsNullOrEmpty(resp.Token));

        // second registration should throw InvalidOperationException
        await Assert.ThrowsAsync<System.InvalidOperationException>(async () => await authService.RegisterAsync(register));
    }
}

// Simple test UnitOfWorkFactory that builds the adapter like other tests
internal class TestUnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IDbTransactionScopeFactory _txFactory;
    private readonly IDbConnectionFactory _connFactory;
    public TestUnitOfWorkFactory(IDbTransactionScopeFactory txFactory, IDbConnectionFactory connFactory)
    {
        _txFactory = txFactory;
        _connFactory = connFactory;
    }

    public IUnitOfWork Create() => new DbUnitOfWorkAdapter(_txFactory, _connFactory);
}
