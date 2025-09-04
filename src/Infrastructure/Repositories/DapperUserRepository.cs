using System.Data;
using Dapper;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;
using SubscriptionTracker.Infrastructure.Dapper;
using SubscriptionTracker.Infrastructure;

namespace SubscriptionTracker.Infrastructure.Repositories;

public class DapperUserRepository : DapperRepositoryBase, IUserRepository
{
    public DapperUserRepository(IDbConnectionFactory db, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null) : base(db, scope) { }

    public async Task AddAsync(UserDto dto, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = @"INSERT INTO users (user_id, email, password_hash) VALUES (@UserId, @Email, @PasswordHash);";
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
        const string sql = "DELETE FROM users WHERE user_id = @Id";
        if (scope is not null)
        {
            await scope.Connection.ExecuteAsync(sql, new { Id = id }, scope.Transaction);
            return;
        }

        using var localScope = BeginTransaction();
        await localScope.Connection.ExecuteAsync(sql, new { Id = id }, localScope.Transaction);
        localScope.Commit();
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        const string sql = @"SELECT user_id AS UserId, email AS Email, password_hash AS PasswordHash FROM users";
        using var conn = CreateConnection();
        return await conn.QueryAsync<UserDto>(sql);
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        const string sql = @"SELECT user_id AS UserId, email AS Email, password_hash AS PasswordHash FROM users WHERE user_id = @Id";
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<UserDto>(sql, new { Id = id });
    }

    public async Task UpdateAsync(UserDto dto, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = @"UPDATE users SET email = @Email, password_hash = @PasswordHash WHERE user_id = @UserId";
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
