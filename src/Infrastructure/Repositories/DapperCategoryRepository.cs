using System.Data;
using Dapper;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;
using SubscriptionTracker.Infrastructure;
using SubscriptionTracker.Infrastructure.Dapper;

namespace SubscriptionTracker.Infrastructure.Repositories;

public class DapperCategoryRepository : DapperRepositoryBase, ICategoryRepository
{
    public DapperCategoryRepository(IDbConnectionFactory db, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null) : base(db, scope) { }

    public async Task AddAsync(CategoryDto dto, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = @"INSERT INTO categories (category_id, user_id, name, created_at)
VALUES (@CategoryId, @UserId, @Name, @CreatedAt);";
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
        const string sql = "DELETE FROM categories WHERE category_id = @Id";
        if (scope is not null)
        {
            await scope.Connection.ExecuteAsync(sql, new { Id = id }, scope.Transaction);
            return;
        }

        using var localScope = BeginTransaction();
        await localScope.Connection.ExecuteAsync(sql, new { Id = id }, localScope.Transaction);
        localScope.Commit();
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        const string sql = @"SELECT category_id AS CategoryId, user_id AS UserId, name AS Name, created_at AS CreatedAt FROM categories";
        using var conn = CreateConnection();
        return await conn.QueryAsync<CategoryDto>(sql);
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id)
    {
        const string sql = @"SELECT category_id AS CategoryId, user_id AS UserId, name AS Name, created_at AS CreatedAt FROM categories WHERE category_id = @Id";
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<CategoryDto>(sql, new { Id = id });
    }

    public async Task UpdateAsync(CategoryDto dto, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = @"UPDATE categories SET user_id = @UserId, name = @Name, created_at = @CreatedAt WHERE category_id = @CategoryId";
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
