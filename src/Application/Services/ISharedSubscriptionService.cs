using SubscriptionTracker.Application.DTOs;

namespace SubscriptionTracker.Application.Services;

public interface ISharedSubscriptionService
{
    Task<IEnumerable<SharedSubscriptionDto>> GetAllAsync();
    Task<SharedSubscriptionDto?> GetByIdAsync(Guid subscriptionId, Guid householdId);
    Task AddAsync(SharedSubscriptionDto dto);
    Task UpdateAsync(SharedSubscriptionDto dto);
    Task DeleteAsync(Guid subscriptionId, Guid householdId);
}
