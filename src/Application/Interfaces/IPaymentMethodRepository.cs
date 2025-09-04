using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;

namespace SubscriptionTracker.Application.Interfaces
{
    public interface IPaymentMethodRepository
    {
        Task<IEnumerable<PaymentMethodDto>> GetAllAsync();
        Task<PaymentMethodDto?> GetByIdAsync(Guid id);
        Task AddAsync(PaymentMethodDto dto, IDbTransactionScope? scope = null);
        Task UpdateAsync(PaymentMethodDto dto, IDbTransactionScope? scope = null);
        Task DeleteAsync(Guid id, IDbTransactionScope? scope = null);
    }
}
