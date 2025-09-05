using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
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

            return await CreateTokenAsync(userEntity);
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

            return await CreateTokenAsync(userEntity);
        }

        private Task<AuthResponse> CreateTokenAsync(Users user)
        {
            var key = _config["JWT:Key"] ?? throw new InvalidOperationException("JWT:Key missing");
            var issuer = _config["JWT:Issuer"] ?? "subscriptiontracker";
            var audience = _config["JWT:Audience"] ?? "subscriptiontracker_clients";
            var expiresMinutes = int.TryParse(_config["JWT:ExpiresMinutes"], out var m) ? m : 60;

            var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()), new Claim(JwtRegisteredClaimNames.Email, user.Email) };
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.UtcNow.AddMinutes(expiresMinutes), signingCredentials: creds);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Task.FromResult(new AuthResponse { Token = tokenString, ExpiresInSeconds = expiresMinutes * 60 });
        }
    }
}
