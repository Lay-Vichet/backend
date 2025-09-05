using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;

namespace SubscriptionTracker.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
