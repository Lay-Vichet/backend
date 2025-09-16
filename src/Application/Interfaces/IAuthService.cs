using SubscriptionTracker.Application.DTOs;

namespace SubscriptionTracker.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Guid> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<RefreshResponse> RefreshAsync(RefreshRequest request);
        Task RevokeRefreshAsync(RevokeRequest request);
    }
}
