using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;
using SubscriptionTracker.Domain.Entities;

namespace SubscriptionTracker.Infrastructure.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IConfiguration _config;

        public AuthService(IUnitOfWorkFactory uowFactory, IConfiguration config)
        {
            _uowFactory = uowFactory;
            _config = config;
        }

        public async Task<Guid> RegisterAsync(RegisterRequest request)
        {
            await using var uow = _uowFactory.Create();
            var existing = await uow.Users.GetByEmailAsync(request.Email);
            if (existing != null)
                throw new InvalidOperationException("Email already in use");

            var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var dto = new UserDto
            {
                UserId = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = hash,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await uow.Users.AddAsync(dto);
                await uow.CommitAsync();
            }
            catch (Npgsql.PostgresException pex) when (pex.SqlState == "23505")
            {
                // unique violation - likely email
                throw new InvalidOperationException("Email already in use");
            }

            var userEntity = new Users
            {
                UserId = dto.UserId,
                Email = dto.Email,
                PasswordHash = dto.PasswordHash,
                CreatedAt = dto.CreatedAt
            };

            return dto.UserId;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            await using var uow = _uowFactory.Create();
            var user = await uow.Users.GetByEmailAsync(request.Email);
            if (user == null) throw new UnauthorizedAccessException("Invalid credentials");

            var ok = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!ok) throw new UnauthorizedAccessException("Invalid credentials");

            var userEntity = new Users
            {
                UserId = user.UserId,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                CreatedAt = user.CreatedAt
            };

            var auth = await CreateTokenAsync(userEntity);
            var refreshToken = GenerateSecureRefreshToken();
            var expires = DateTime.UtcNow.AddDays(14);

            // persist refresh token using UnitOfWork's repository bound to same scope
            await uow.RefreshTokens.CreateAsync(user.UserId, refreshToken, expires);
            await uow.CommitAsync();

            return new AuthResponse { Token = auth.Token, ExpiresInSeconds = auth.ExpiresInSeconds, RefreshToken = refreshToken };
        }

        public async Task<RefreshResponse> RefreshAsync(RefreshRequest request)
        {
            // basic rotation: validate existing token, create new token, revoke old
            var raw = request.RefreshToken;
            if (string.IsNullOrEmpty(raw)) throw new UnauthorizedAccessException();

            await using var uow = _uowFactory.Create();
            var rec = await uow.RefreshTokens.GetByHashAsync(raw);
            if (rec == null || rec.Revoked || rec.ExpiresAt < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException();
            }

            // rotate
            var newRaw = GenerateSecureRefreshToken();
            var newExpires = DateTime.UtcNow.AddDays(14);
            await uow.RefreshTokens.CreateAsync(rec.UserId, newRaw, newExpires);
            await uow.RefreshTokens.RevokeAsync(rec.Id);
            await uow.CommitAsync();

            // generate new access token
            var userDto = await uow.Users.GetByIdAsync(rec.UserId);
            if (userDto == null) throw new UnauthorizedAccessException();
            var userEntity = new Users { UserId = userDto.UserId, Email = userDto.Email, PasswordHash = userDto.PasswordHash, CreatedAt = userDto.CreatedAt };
            var access = await CreateTokenAsync(userEntity);
            return new RefreshResponse(access.Token, newRaw, access.ExpiresInSeconds);
        }

        public async Task RevokeRefreshAsync(RevokeRequest request)
        {
            var raw = request.RefreshToken;
            if (string.IsNullOrEmpty(raw)) return;
            await using var uow = _uowFactory.Create();
            var rec = await uow.RefreshTokens.GetByHashAsync(raw);
            if (rec != null)
            {
                await uow.RefreshTokens.RevokeAsync(rec.Id);
            }
        }

        private Task<AuthResponse> CreateTokenAsync(Users user)
        {
            var key = _config["JWT:Key"] ?? throw new InvalidOperationException("JWT:Key missing");
            var issuer = _config["JWT:Issuer"] ?? "subscriptiontracker";
            var audience = _config["JWT:Audience"] ?? "subscriptiontracker_clients";
            var expiresMinutes = int.TryParse(_config["JWT:ExpiresMinutes"], out var m) ? m : 60;

            var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()), new Claim(JwtRegisteredClaimNames.Email, user.Email) };
            byte[] signingKeyBytes;
            if (key.IndexOfAny(new[] { '+', '/', '=' }) >= 0)
            {
                try { signingKeyBytes = Convert.FromBase64String(key); }
                catch { signingKeyBytes = Encoding.UTF8.GetBytes(key); }
            }
            else
            {
                signingKeyBytes = Encoding.UTF8.GetBytes(key);
            }

            // Ensure key meets HS256 minimum size (256 bits). If it's shorter, derive a 256-bit key via SHA-256.
            if (signingKeyBytes.Length < 32)
            {
                signingKeyBytes = SHA256.HashData(signingKeyBytes);
            }

            var signingKey = new SymmetricSecurityKey(signingKeyBytes);
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.UtcNow.AddMinutes(expiresMinutes), signingCredentials: creds);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Task.FromResult(new AuthResponse { Token = tokenString, ExpiresInSeconds = expiresMinutes * 60 });
        }

        private static string GenerateSecureRefreshToken(int sizeBytes = 64)
        {
            var data = new byte[sizeBytes];
            RandomNumberGenerator.Fill(data);
            return Convert.ToBase64String(data);
        }
    }
}
