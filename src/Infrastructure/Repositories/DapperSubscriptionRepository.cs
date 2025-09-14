using Dapper;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;
using SubscriptionTracker.Infrastructure.Dapper;

namespace SubscriptionTracker.Infrastructure.Repositories;

public class DapperSubscriptionRepository : DapperRepositoryBase, ISubscriptionRepository
{
    public DapperSubscriptionRepository(IDbConnectionFactory db, IDbTransactionScope? scope = null) : base(db, scope) { }

    public async Task AddAsync(SubscriptionDto subscription, IDbTransactionScope? scope = null)
    {
        const string sql = @"INSERT INTO subscriptions (subscription_id, user_id, name, cost, currency, billing_cycle, start_date, next_due_date, notes, created_at)
VALUES (@Id, @UserId, @Name, @Cost, @Currency, @BillingCycle, @StartDate, @NextDueDate, @Notes, @CreatedAt);";
        var effectiveScope = scope ?? RepositoryTransactionScope;
        if (effectiveScope is not null)
        {
            await effectiveScope.Connection.ExecuteAsync(sql, subscription, effectiveScope.Transaction);
            return;
        }

        using var localScope = BeginTransaction();
        await localScope.Connection.ExecuteAsync(sql, subscription, localScope.Transaction);
        localScope.Commit();
    }

    public async Task DeleteAsync(Guid id, Guid userId, IDbTransactionScope? scope = null)
    {
        const string sql = "DELETE FROM subscriptions WHERE subscription_id = @Id AND user_id = @UserId";
        var effectiveScope = scope ?? RepositoryTransactionScope;
        if (effectiveScope is not null)
        {
            await effectiveScope.Connection.ExecuteAsync(sql, new { Id = id, UserId = userId }, effectiveScope.Transaction);
            return;
        }

        using var localScope = BeginTransaction();
        await localScope.Connection.ExecuteAsync(sql, new { Id = id, UserId = userId }, localScope.Transaction);
        localScope.Commit();
    }

    public async Task<IEnumerable<SubscriptionDto>> GetAllAsync(Guid userId)
    {
        const string sql = @"SELECT subscription_id AS Id, name AS Name, cost AS Cost, currency AS Currency, billing_cycle AS BillingCycle, start_date AS StartDate, next_due_date AS NextDueDate, notes AS Notes
FROM subscriptions WHERE user_id = @UserId";
        using var conn = CreateConnection();
        return await conn.QueryAsync<SubscriptionDto>(sql, new { UserId = userId });
    }

    public async Task<SubscriptionDto?> GetByIdAsync(Guid id, Guid userId)
    {
        const string sql = @"SELECT subscription_id AS Id, name AS Name, cost AS Cost, currency AS Currency, billing_cycle AS BillingCycle, start_date AS StartDate, next_due_date AS NextDueDate, notes AS Notes
FROM subscriptions WHERE subscription_id = @Id AND user_id = @UserId";
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<SubscriptionDto>(sql, new { Id = id, UserId = userId });
    }

    public async Task UpdateAsync(SubscriptionDto subscription, IDbTransactionScope? scope = null)
    {
        const string sql = @"UPDATE subscriptions SET name = @Name, cost = @Cost, currency = @Currency, billing_cycle = @BillingCycle, start_date = @StartDate, next_due_date = @NextDueDate, notes = @Notes
WHERE subscription_id = @Id AND user_id = @UserId";
        var effectiveScope = scope ?? RepositoryTransactionScope;
        if (effectiveScope is not null)
        {
            await effectiveScope.Connection.ExecuteAsync(sql, subscription, effectiveScope.Transaction);
            return;
        }

        using var localScope = BeginTransaction();
        await localScope.Connection.ExecuteAsync(sql, subscription, localScope.Transaction);
        localScope.Commit();
    }
}
