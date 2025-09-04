using SubscriptionTracker.Application.DTOs;

namespace SubscriptionTracker.Application.Services;

public interface ISubscriptionRatingService
{
    Task<IEnumerable<SubscriptionRatingDto>> GetAllAsync();
    Task<SubscriptionRatingDto?> GetByIdAsync(Guid id);
    Task AddAsync(SubscriptionRatingDto dto);
    Task UpdateAsync(SubscriptionRatingDto dto);
    Task DeleteAsync(Guid id);
}
