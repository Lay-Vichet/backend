using SubscriptionTracker.Application.DTOs;

namespace SubscriptionTracker.Application.Services;

public interface ISubscriptionPaymentService
{
    Task<IEnumerable<SubscriptionPaymentDto>> GetAllAsync();
    Task<SubscriptionPaymentDto?> GetByIdAsync(Guid id);
    Task AddAsync(SubscriptionPaymentDto dto);
    Task UpdateAsync(SubscriptionPaymentDto dto);
    Task DeleteAsync(Guid id);
    Task AddPaymentAndUpdateSubscriptionAsync(SubscriptionPaymentDto payment, SubscriptionTracker.Application.DTOs.SubscriptionDto updatedSubscription);
}
