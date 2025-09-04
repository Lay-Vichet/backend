using System.Data;
using Dapper;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;
using SubscriptionTracker.Infrastructure.Dapper;
using SubscriptionTracker.Infrastructure;

namespace SubscriptionTracker.Infrastructure.Repositories;

public class DapperHouseholdRepository : DapperRepositoryBase, IHouseholdRepository
{
    public DapperHouseholdRepository(IDbConnectionFactory db, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null) : base(db, scope) { }

    public async Task AddAsync(HouseholdDto dto, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = @"INSERT INTO households (household_id, name) VALUES (@HouseholdId, @Name);";
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
        const string sql = "DELETE FROM households WHERE household_id = @Id";
        if (scope is not null)
        {
            await scope.Connection.ExecuteAsync(sql, new { Id = id }, scope.Transaction);
            return;
        }

        using var localScope = BeginTransaction();
        await localScope.Connection.ExecuteAsync(sql, new { Id = id }, localScope.Transaction);
        localScope.Commit();
    }

    public async Task<IEnumerable<HouseholdDto>> GetAllAsync()
    {
        const string sql = @"SELECT household_id AS HouseholdId, name AS Name FROM households";
        using var conn = CreateConnection();
        return await conn.QueryAsync<HouseholdDto>(sql);
    }

    public async Task<HouseholdDto?> GetByIdAsync(Guid id)
    {
        const string sql = @"SELECT household_id AS HouseholdId, name AS Name FROM households WHERE household_id = @Id";
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<HouseholdDto>(sql, new { Id = id });
    }

    public async Task UpdateAsync(HouseholdDto dto, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = @"UPDATE households SET name = @Name WHERE household_id = @HouseholdId";
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
