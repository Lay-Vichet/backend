namespace SubscriptionTracker.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task CreateAsync(Guid userId, string rawToken, DateTime expiresAt);
        Task<DTOs.RefreshTokenDto?> GetByHashAsync(string rawToken);
        Task RevokeAsync(Guid id);
        Task RevokeAllForUserAsync(Guid userId);
    }
}
