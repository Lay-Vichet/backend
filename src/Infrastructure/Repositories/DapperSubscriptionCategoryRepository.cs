using System.Data;
using Dapper;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;
using SubscriptionTracker.Infrastructure.Dapper;
using SubscriptionTracker.Infrastructure;

namespace SubscriptionTracker.Infrastructure.Repositories;

public class DapperSubscriptionCategoryRepository : DapperRepositoryBase, ISubscriptionCategoryRepository
{
    public DapperSubscriptionCategoryRepository(IDbConnectionFactory db, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null) : base(db, scope) { }

    public async Task AddAsync(SubscriptionCategoryDto dto, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = @"INSERT INTO subscription_categories (subscription_id, category_id) VALUES (@SubscriptionId, @CategoryId);";
        if (scope is not null)
        {
            await scope.Connection.ExecuteAsync(sql, dto, scope.Transaction);
            return;
        }

        using var localScope = BeginTransaction();
        await localScope.Connection.ExecuteAsync(sql, dto, localScope.Transaction);
        localScope.Commit();
    }

    public async Task DeleteAsync(Guid subscriptionId, Guid categoryId, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = "DELETE FROM subscription_categories WHERE subscription_id = @SubscriptionId AND category_id = @CategoryId";
        if (scope is not null)
        {
            await scope.Connection.ExecuteAsync(sql, new { SubscriptionId = subscriptionId, CategoryId = categoryId }, scope.Transaction);
            return;
        }

        using var localScope = BeginTransaction();
        await localScope.Connection.ExecuteAsync(sql, new { SubscriptionId = subscriptionId, CategoryId = categoryId }, localScope.Transaction);
        localScope.Commit();
    }

    public async Task<IEnumerable<SubscriptionCategoryDto>> GetAllAsync()
    {
        const string sql = @"SELECT subscription_id AS SubscriptionId, category_id AS CategoryId FROM subscription_categories";
        using var conn = CreateConnection();
        return await conn.QueryAsync<SubscriptionCategoryDto>(sql);
    }

    public async Task<SubscriptionCategoryDto?> GetByIdAsync(Guid subscriptionId, Guid categoryId)
    {
        const string sql = @"SELECT subscription_id AS SubscriptionId, category_id AS CategoryId FROM subscription_categories WHERE subscription_id = @SubscriptionId AND category_id = @CategoryId";
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<SubscriptionCategoryDto>(sql, new { SubscriptionId = subscriptionId, CategoryId = categoryId });
    }

    public async Task UpdateAsync(SubscriptionCategoryDto dto, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        // No-op; this is just a link table. Replace with explicit logic if needed.
        await Task.CompletedTask;
    }
}
