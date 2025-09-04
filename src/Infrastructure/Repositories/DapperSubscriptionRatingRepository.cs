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

public class DapperSubscriptionRatingRepository : DapperRepositoryBase, ISubscriptionRatingRepository
{
    public DapperSubscriptionRatingRepository(IDbConnectionFactory db, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null) : base(db, scope) { }

    public async Task AddAsync(SubscriptionRatingDto dto, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = @"INSERT INTO subscription_ratings (rating_id, subscription_id, rating, rated_at) VALUES (@RatingId, @SubscriptionId, @Rating, @RatedAt);";
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
        const string sql = "DELETE FROM subscription_ratings WHERE rating_id = @Id";
        if (scope is not null)
        {
            await scope.Connection.ExecuteAsync(sql, new { Id = id }, scope.Transaction);
            return;
        }

        using var localScope = BeginTransaction();
        await localScope.Connection.ExecuteAsync(sql, new { Id = id }, localScope.Transaction);
        localScope.Commit();
    }

    public async Task<IEnumerable<SubscriptionRatingDto>> GetAllAsync()
    {
        const string sql = @"SELECT rating_id AS RatingId, subscription_id AS SubscriptionId, rating AS Rating, rated_at AS RatedAt FROM subscription_ratings";
        using var conn = CreateConnection();
        return await conn.QueryAsync<SubscriptionRatingDto>(sql);
    }

    public async Task<SubscriptionRatingDto?> GetByIdAsync(Guid id)
    {
        const string sql = @"SELECT rating_id AS RatingId, subscription_id AS SubscriptionId, rating AS Rating, rated_at AS RatedAt FROM subscription_ratings WHERE rating_id = @Id";
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<SubscriptionRatingDto>(sql, new { Id = id });
    }

    public async Task UpdateAsync(SubscriptionRatingDto dto, SubscriptionTracker.Application.Interfaces.IDbTransactionScope? scope = null)
    {
        const string sql = @"UPDATE subscription_ratings SET subscription_id = @SubscriptionId, rating = @Rating, rated_at = @RatedAt WHERE rating_id = @RatingId";
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
