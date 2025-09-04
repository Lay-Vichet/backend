using SubscriptionTracker.Application.DTOs;

namespace SubscriptionTracker.Application.Services;

public interface IHouseholdService
{
    Task<IEnumerable<HouseholdDto>> GetAllAsync();
    Task<HouseholdDto?> GetByIdAsync(Guid id);
    Task AddAsync(HouseholdDto dto);
    Task UpdateAsync(HouseholdDto dto);
    Task DeleteAsync(Guid id);
}
