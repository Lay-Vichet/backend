using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;
using SubscriptionTracker.Infrastructure.Dapper;
using SubscriptionTracker.Infrastructure;

namespace SubscriptionTracker.Infrastructure.Repositories;

public class DapperSubscriptionRepository : DapperRepositoryBase, ISubscriptionRepository
{
    public DapperSubscriptionRepository(IDbConnectionFactory db, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null) : base(db, scope) { }

    public async Task AddAsync(SubscriptionDto subscription, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = @"INSERT INTO subscriptions (id, name, monthly_cost, start_date)
VALUES (@Id, @Name, @MonthlyCost, @StartDate);";
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

    public async Task DeleteAsync(Guid id, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = "DELETE FROM subscriptions WHERE id = @Id";
        var effectiveScope = scope ?? RepositoryTransactionScope;
        if (effectiveScope is not null)
        {
            await effectiveScope.Connection.ExecuteAsync(sql, new { Id = id }, effectiveScope.Transaction);
            return;
        }

        using var localScope = BeginTransaction();
        await localScope.Connection.ExecuteAsync(sql, new { Id = id }, localScope.Transaction);
        localScope.Commit();
    }

    public async Task<IEnumerable<SubscriptionDto>> GetAllAsync()
    {
        const string sql = @"SELECT id AS Id, name AS Name, monthly_cost AS MonthlyCost, start_date AS StartDate
FROM subscriptions";
        using var conn = CreateConnection();
        return await conn.QueryAsync<SubscriptionDto>(sql);
    }

    public async Task<SubscriptionDto?> GetByIdAsync(Guid id)
    {
        const string sql = @"SELECT id AS Id, name AS Name, monthly_cost AS MonthlyCost, start_date AS StartDate
FROM subscriptions WHERE id = @Id";
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<SubscriptionDto>(sql, new { Id = id });
    }

    public async Task UpdateAsync(SubscriptionDto subscription, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = @"UPDATE subscriptions SET name = @Name, monthly_cost = @MonthlyCost, start_date = @StartDate
WHERE id = @Id";
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
