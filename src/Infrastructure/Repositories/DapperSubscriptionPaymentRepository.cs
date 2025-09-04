using System.Data;
using Dapper;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;
using SubscriptionTracker.Infrastructure.Dapper;
using SubscriptionTracker.Infrastructure;

namespace SubscriptionTracker.Infrastructure.Repositories;

public class DapperSubscriptionPaymentRepository : DapperRepositoryBase, ISubscriptionPaymentRepository
{
    public DapperSubscriptionPaymentRepository(IDbConnectionFactory db, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null) : base(db, scope) { }

    public async Task AddAsync(SubscriptionPaymentDto dto, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = @"INSERT INTO subscription_payments (payment_id, subscription_id, amount, paid_at) VALUES (@PaymentId, @SubscriptionId, @Amount, @PaidAt);";
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
        const string sql = "DELETE FROM subscription_payments WHERE payment_id = @Id";
        if (scope is not null)
        {
            await scope.Connection.ExecuteAsync(sql, new { Id = id }, scope.Transaction);
            return;
        }

        using var localScope = BeginTransaction();
        await localScope.Connection.ExecuteAsync(sql, new { Id = id }, localScope.Transaction);
        localScope.Commit();
    }

    public async Task<IEnumerable<SubscriptionPaymentDto>> GetAllAsync()
    {
        const string sql = @"SELECT payment_id AS PaymentId, subscription_id AS SubscriptionId, amount AS Amount, paid_at AS PaidAt FROM subscription_payments";
        using var conn = CreateConnection();
        return await conn.QueryAsync<SubscriptionPaymentDto>(sql);
    }

    public async Task<SubscriptionPaymentDto?> GetByIdAsync(Guid id)
    {
        const string sql = @"SELECT payment_id AS PaymentId, subscription_id AS SubscriptionId, amount AS Amount, paid_at AS PaidAt FROM subscription_payments WHERE payment_id = @Id";
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<SubscriptionPaymentDto>(sql, new { Id = id });
    }

    public async Task UpdateAsync(SubscriptionPaymentDto dto, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = @"UPDATE subscription_payments SET subscription_id = @SubscriptionId, amount = @Amount, paid_at = @PaidAt WHERE payment_id = @PaymentId";
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
