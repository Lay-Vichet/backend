using SubscriptionTracker.Application.DTOs;

namespace SubscriptionTracker.Application.Services;

public interface IPaymentMethodService
{
    Task<IEnumerable<PaymentMethodDto>> GetAllAsync();
    Task<PaymentMethodDto?> GetByIdAsync(Guid id);
    Task AddAsync(PaymentMethodDto dto);
    Task UpdateAsync(PaymentMethodDto dto);
    Task DeleteAsync(Guid id);
}
