using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;

namespace SubscriptionTracker.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Guid> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
