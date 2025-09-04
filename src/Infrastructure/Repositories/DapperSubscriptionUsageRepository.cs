using System.Data;
using Dapper;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;
using SubscriptionTracker.Infrastructure.Dapper;
using SubscriptionTracker.Infrastructure;

namespace SubscriptionTracker.Infrastructure.Repositories;

public class DapperSubscriptionUsageRepository : DapperRepositoryBase, ISubscriptionUsageRepository
{
    public DapperSubscriptionUsageRepository(IDbConnectionFactory db, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null) : base(db, scope) { }

    public async Task AddAsync(SubscriptionUsageDto dto, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = @"INSERT INTO subscription_usage (usage_id, subscription_id, usage_count, recorded_at) VALUES (@UsageId, @SubscriptionId, @UsageCount, @RecordedAt);";
        if (scope is not null)
        {
            await scope.Connection.ExecuteAsync(sql, dto, scope.Transaction);
            return;
        }

        using var localScope = BeginTransaction();
        await localScope.Connection.ExecuteAsync(sql, dto, localScope.Transaction);
        localScope.Commit();
    }

    public async Task DeleteAsync(Guid id, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = "DELETE FROM subscription_usage WHERE usage_id = @Id";
        if (scope is not null)
        {
            await scope.Connection.ExecuteAsync(sql, new { Id = id }, scope.Transaction);
            return;
        }

        using var localScope = BeginTransaction();
        await localScope.Connection.ExecuteAsync(sql, new { Id = id }, localScope.Transaction);
        localScope.Commit();
    }

    public async Task<IEnumerable<SubscriptionUsageDto>> GetAllAsync()
    {
        const string sql = @"SELECT usage_id AS UsageId, subscription_id AS SubscriptionId, usage_count AS UsageCount, recorded_at AS RecordedAt FROM subscription_usage";
        using var conn = CreateConnection();
        return await conn.QueryAsync<SubscriptionUsageDto>(sql);
    }

    public async Task<SubscriptionUsageDto?> GetByIdAsync(Guid id)
    {
        const string sql = @"SELECT usage_id AS UsageId, subscription_id AS SubscriptionId, usage_count AS UsageCount, recorded_at AS RecordedAt FROM subscription_usage WHERE usage_id = @Id";
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<SubscriptionUsageDto>(sql, new { Id = id });
    }

    public async Task UpdateAsync(SubscriptionUsageDto dto, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = @"UPDATE subscription_usage SET subscription_id = @SubscriptionId, usage_count = @UsageCount, recorded_at = @RecordedAt WHERE usage_id = @UsageId";
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
