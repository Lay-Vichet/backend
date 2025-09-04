using System.Data;
using Dapper;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;
using SubscriptionTracker.Infrastructure.Dapper;
using SubscriptionTracker.Infrastructure;

namespace SubscriptionTracker.Infrastructure.Repositories;

public class DapperSharedSubscriptionRepository : DapperRepositoryBase, ISharedSubscriptionRepository
{
    public DapperSharedSubscriptionRepository(IDbConnectionFactory db, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null) : base(db, scope) { }

    public async Task AddAsync(SharedSubscriptionDto dto, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = @"INSERT INTO shared_subscriptions (subscription_id, household_id, shared_at) VALUES (@SubscriptionId, @HouseholdId, @SharedAt);";
        if (scope is not null)
        {
            await scope.Connection.ExecuteAsync(sql, dto, scope.Transaction);
            return;
        }

        using var localScope = BeginTransaction();
        await localScope.Connection.ExecuteAsync(sql, dto, localScope.Transaction);
        localScope.Commit();
    }

    public async Task DeleteAsync(Guid subscriptionId, Guid householdId, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = "DELETE FROM shared_subscriptions WHERE subscription_id = @SubscriptionId AND household_id = @HouseholdId";
        if (scope is not null)
        {
            await scope.Connection.ExecuteAsync(sql, new { SubscriptionId = subscriptionId, HouseholdId = householdId }, scope.Transaction);
            return;
        }

        using var localScope = BeginTransaction();
        await localScope.Connection.ExecuteAsync(sql, new { SubscriptionId = subscriptionId, HouseholdId = householdId }, localScope.Transaction);
        localScope.Commit();
    }

    public async Task<IEnumerable<SharedSubscriptionDto>> GetAllAsync()
    {
        const string sql = @"SELECT subscription_id AS SubscriptionId, household_id AS HouseholdId, shared_at AS SharedAt FROM shared_subscriptions";
        using var conn = CreateConnection();
        return await conn.QueryAsync<SharedSubscriptionDto>(sql);
    }

    public async Task<SharedSubscriptionDto?> GetByIdAsync(Guid subscriptionId, Guid householdId)
    {
        const string sql = @"SELECT subscription_id AS SubscriptionId, household_id AS HouseholdId, shared_at AS SharedAt FROM shared_subscriptions WHERE subscription_id = @SubscriptionId AND household_id = @HouseholdId";
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<SharedSubscriptionDto>(sql, new { SubscriptionId = subscriptionId, HouseholdId = householdId });
    }

    public async Task UpdateAsync(SharedSubscriptionDto dto, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = @"UPDATE shared_subscriptions SET shared_at = @SharedAt WHERE subscription_id = @SubscriptionId AND household_id = @HouseholdId";
        if (scope is not null)
        {
            await scope.Connection.ExecuteAsync(sql, dto, scope.Transaction);
            return;
        }

        using var localScope = BeginTransaction();
        await localScope.Connection.ExecuteAsync(sql, dto, localScope.Transaction);
        localScope.Commit();
    }
}
