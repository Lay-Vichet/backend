using SubscriptionTracker.Application.DTOs;

namespace SubscriptionTracker.Application.Services;

public interface ISubscriptionCategoryService
{
    Task<IEnumerable<SubscriptionCategoryDto>> GetAllAsync();
    Task<SubscriptionCategoryDto?> GetByIdAsync(Guid subscriptionId, Guid categoryId);
    Task AddAsync(SubscriptionCategoryDto dto);
    Task UpdateAsync(SubscriptionCategoryDto dto);
    Task DeleteAsync(Guid subscriptionId, Guid categoryId);
}
