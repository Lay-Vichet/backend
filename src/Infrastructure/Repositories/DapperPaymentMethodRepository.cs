using System.Data;
using Dapper;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;
using SubscriptionTracker.Infrastructure.Dapper;
using SubscriptionTracker.Infrastructure;

namespace SubscriptionTracker.Infrastructure.Repositories;

public class DapperPaymentMethodRepository : DapperRepositoryBase, IPaymentMethodRepository
{
    public DapperPaymentMethodRepository(IDbConnectionFactory db, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null) : base(db, scope) { }

    public async Task AddAsync(PaymentMethodDto dto, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = @"INSERT INTO payment_methods (payment_method_id, user_id, card_brand, last_four, display_name) VALUES (@PaymentMethodId, @UserId, @CardBrand, @LastFour, @DisplayName);";
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
        const string sql = "DELETE FROM payment_methods WHERE payment_method_id = @Id";
        if (scope is not null)
        {
            await scope.Connection.ExecuteAsync(sql, new { Id = id }, scope.Transaction);
            return;
        }

        using var localScope = BeginTransaction();
        await localScope.Connection.ExecuteAsync(sql, new { Id = id }, localScope.Transaction);
        localScope.Commit();
    }

    public async Task<IEnumerable<PaymentMethodDto>> GetAllAsync()
    {
        const string sql = @"SELECT payment_method_id AS PaymentMethodId, user_id AS UserId, card_brand AS CardBrand, last_four AS LastFour, display_name AS DisplayName FROM payment_methods";
        using var conn = CreateConnection();
        return await conn.QueryAsync<PaymentMethodDto>(sql);
    }

    public async Task<PaymentMethodDto?> GetByIdAsync(Guid id)
    {
        const string sql = @"SELECT payment_method_id AS PaymentMethodId, user_id AS UserId, card_brand AS CardBrand, last_four AS LastFour, display_name AS DisplayName FROM payment_methods WHERE payment_method_id = @Id";
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<PaymentMethodDto>(sql, new { Id = id });
    }

    public async Task UpdateAsync(PaymentMethodDto dto, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = @"UPDATE payment_methods SET user_id = @UserId, card_brand = @CardBrand, last_four = @LastFour, display_name = @DisplayName WHERE payment_method_id = @PaymentMethodId";
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
