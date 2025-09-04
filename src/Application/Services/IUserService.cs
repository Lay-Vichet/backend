using SubscriptionTracker.Application.DTOs;

namespace SubscriptionTracker.Application.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(Guid id);
    Task AddAsync(UserDto dto);
    Task UpdateAsync(UserDto dto);
    Task DeleteAsync(Guid id);
}
