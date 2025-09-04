using SubscriptionTracker.Application.DTOs;

namespace SubscriptionTracker.Application.Services;

public interface ISubscriptionUsageService
{
    Task<IEnumerable<SubscriptionUsageDto>> GetAllAsync();
    Task<SubscriptionUsageDto?> GetByIdAsync(Guid id);
    Task AddAsync(SubscriptionUsageDto dto);
    Task UpdateAsync(SubscriptionUsageDto dto);
    Task DeleteAsync(Guid id);
}
