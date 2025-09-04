using SubscriptionTracker.Application.DTOs;

namespace SubscriptionTracker.Application.Services;

public interface IHouseholdMemberService
{
    Task<IEnumerable<HouseholdMemberDto>> GetAllAsync();
    Task<HouseholdMemberDto?> GetByIdAsync(Guid householdId, Guid userId);
    Task AddAsync(HouseholdMemberDto dto);
    Task UpdateAsync(HouseholdMemberDto dto);
    Task DeleteAsync(Guid householdId, Guid userId);
}
