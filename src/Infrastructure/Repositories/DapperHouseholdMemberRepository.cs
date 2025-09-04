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

public class DapperHouseholdMemberRepository : DapperRepositoryBase, IHouseholdMemberRepository
{
    public DapperHouseholdMemberRepository(IDbConnectionFactory db, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null) : base(db, scope) { }

    public async Task AddAsync(HouseholdMemberDto dto, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = @"INSERT INTO household_members (household_id, user_id, role) VALUES (@HouseholdId, @UserId, @Role);";
        if (scope is not null)
        {
            await scope.Connection.ExecuteAsync(sql, dto, scope.Transaction);
            return;
        }

        using var localScope = BeginTransaction();
        await localScope.Connection.ExecuteAsync(sql, dto, localScope.Transaction);
        localScope.Commit();
    }

    public async Task DeleteAsync(Guid householdId, Guid userId, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = "DELETE FROM household_members WHERE household_id = @HouseholdId AND user_id = @UserId";
        if (scope is not null)
        {
            await scope.Connection.ExecuteAsync(sql, new { HouseholdId = householdId, UserId = userId }, scope.Transaction);
            return;
        }

        using var localScope = BeginTransaction();
        await localScope.Connection.ExecuteAsync(sql, new { HouseholdId = householdId, UserId = userId }, localScope.Transaction);
        localScope.Commit();
    }

    public async Task<IEnumerable<HouseholdMemberDto>> GetAllAsync()
    {
        const string sql = @"SELECT household_id AS HouseholdId, user_id AS UserId, role AS Role FROM household_members";
        using var conn = CreateConnection();
        return await conn.QueryAsync<HouseholdMemberDto>(sql);
    }

    public async Task<HouseholdMemberDto?> GetByIdAsync(Guid householdId, Guid userId)
    {
        const string sql = @"SELECT household_id AS HouseholdId, user_id AS UserId, role AS Role FROM household_members WHERE household_id = @HouseholdId AND user_id = @UserId";
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<HouseholdMemberDto>(sql, new { HouseholdId = householdId, UserId = userId });
    }

    public async Task UpdateAsync(HouseholdMemberDto dto, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = @"UPDATE household_members SET role = @Role WHERE household_id = @HouseholdId AND user_id = @UserId";
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
