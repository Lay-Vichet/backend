using SubscriptionTracker.Application.DTOs;

namespace SubscriptionTracker.Application.Services
{
    public interface ISubscriptionService
    {
        Task<IEnumerable<SubscriptionDto>> GetAllAsync();
        Task<SubscriptionDto?> GetByIdAsync(Guid id);
        Task<SubscriptionDto> CreateAsync(SubscriptionDto dto);
        Task UpdateAsync(Guid id, SubscriptionDto dto);
        Task DeleteAsync(Guid id);
    }
}
