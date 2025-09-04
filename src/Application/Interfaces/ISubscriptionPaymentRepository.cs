using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;

namespace SubscriptionTracker.Application.Interfaces
{
    public interface ISubscriptionPaymentRepository
    {
        Task<IEnumerable<SubscriptionPaymentDto>> GetAllAsync();
        Task<SubscriptionPaymentDto?> GetByIdAsync(Guid id);
        Task AddAsync(SubscriptionPaymentDto dto, IDbTransactionScope? scope = null);
        Task UpdateAsync(SubscriptionPaymentDto dto, IDbTransactionScope? scope = null);
        Task DeleteAsync(Guid id, IDbTransactionScope? scope = null);
    }
}
