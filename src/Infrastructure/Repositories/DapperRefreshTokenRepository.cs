using System.Data;
using Dapper;
using System.Security.Cryptography;
using System.Text;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Infrastructure.Repositories
{
    public class RefreshTokenRecord
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool Revoked { get; set; }
        public Guid? ReplacedByTokenId { get; set; }
    }

    public class DapperRefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IDbConnectionFactory _db;
        private readonly IDbTransactionScope? _scope;

        public DapperRefreshTokenRepository(IDbConnectionFactory db, IDbTransactionScope? scope = null)
        {
            _db = db;
            _scope = scope;
        }

        private IDbConnection CreateConnection()
        {
            var c = _db.CreateConnection();
            if (c.State != ConnectionState.Open) c.Open();
            return c;
        }

        private static string HashToken(string token)
        {
            var bytes = Encoding.UTF8.GetBytes(token);
            var hashed = SHA256.HashData(bytes);
            return Convert.ToHexString(hashed);
        }

        public async Task CreateAsync(Guid userId, string rawToken, DateTime expiresAt)
        {
            const string sql = @"INSERT INTO refresh_tokens (id, user_id, token_hash, created_at, expires_at, revoked)
VALUES (@Id, @UserId, @TokenHash, @CreatedAt, @ExpiresAt, false);";
            var rec = new
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TokenHash = HashToken(rawToken),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt
            };
            var effectiveScope = _scope;
            if (effectiveScope is not null)
            {
                await effectiveScope.Connection.ExecuteAsync(sql, rec, effectiveScope.Transaction);
                return;
            }

            using var conn = CreateConnection();
            await conn.ExecuteAsync(sql, rec);
        }

        public async Task<Application.DTOs.RefreshTokenDto?> GetByHashAsync(string rawToken)
        {
            const string sql = @"SELECT id AS Id, user_id AS UserId, token_hash AS TokenHash, created_at AS CreatedAt, expires_at AS ExpiresAt, revoked AS Revoked, replaced_by_token_id AS ReplacedByTokenId FROM refresh_tokens WHERE token_hash = @TokenHash";
            var hash = HashToken(rawToken);
            var effectiveScope = _scope;
            RefreshTokenRecord? rec = null;
            if (effectiveScope is not null)
            {
                rec = await effectiveScope.Connection.QueryFirstOrDefaultAsync<RefreshTokenRecord>(sql, new { TokenHash = hash }, effectiveScope.Transaction);
            }
            else
            {
                using var conn = CreateConnection();
                rec = await conn.QueryFirstOrDefaultAsync<RefreshTokenRecord>(sql, new { TokenHash = hash });
            }

            if (rec == null) return null;
            return new Application.DTOs.RefreshTokenDto
            {
                Id = rec.Id,
                UserId = rec.UserId,
                CreatedAt = rec.CreatedAt,
                ExpiresAt = rec.ExpiresAt,
                Revoked = rec.Revoked,
                ReplacedByTokenId = rec.ReplacedByTokenId
            };
        }

        public async Task RevokeAsync(Guid id)
        {
            const string sql = "UPDATE refresh_tokens SET revoked = true, revoked_at = now() WHERE id = @Id";
            var effectiveScope = _scope;
            if (effectiveScope is not null)
            {
                await effectiveScope.Connection.ExecuteAsync(sql, new { Id = id }, effectiveScope.Transaction);
                return;
            }

            using var conn = CreateConnection();
            await conn.ExecuteAsync(sql, new { Id = id });
        }

        public async Task RevokeAllForUserAsync(Guid userId)
        {
            const string sql = "UPDATE refresh_tokens SET revoked = true, revoked_at = now() WHERE user_id = @UserId AND revoked = false";
            using var conn = CreateConnection();
            await conn.ExecuteAsync(sql, new { UserId = userId });
        }
    }
}
