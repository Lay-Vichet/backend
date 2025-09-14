using SubscriptionTracker.Application.DTOs;

namespace SubscriptionTracker.Application.Services
{
    public interface ISubscriptionService
    {
        Task<IEnumerable<SubscriptionDto>> GetAllAsync(Guid userId);
        Task<SubscriptionDto?> GetByIdAsync(Guid id, Guid userId);
        Task<SubscriptionDto> CreateAsync(SubscriptionDto dto);
        Task UpdateAsync(Guid id, SubscriptionDto dto);
        Task DeleteAsync(Guid id, Guid userId);
    }
}
