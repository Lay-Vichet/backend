using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;

namespace SubscriptionTracker.Application.Interfaces
{
    public interface ISharedSubscriptionRepository
    {
        Task<IEnumerable<SharedSubscriptionDto>> GetAllAsync();
        Task<SharedSubscriptionDto?> GetByIdAsync(Guid subscriptionId, Guid householdId);
        Task AddAsync(SharedSubscriptionDto dto, IDbTransactionScope? scope = null);
        Task UpdateAsync(SharedSubscriptionDto dto, IDbTransactionScope? scope = null);
        Task DeleteAsync(Guid subscriptionId, Guid householdId, IDbTransactionScope? scope = null);
    }
}
