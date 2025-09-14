using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;

namespace SubscriptionTracker.Application.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task<IEnumerable<SubscriptionDto>> GetAllAsync(Guid userId);
        Task<SubscriptionDto?> GetByIdAsync(Guid id, Guid userId);
        Task AddAsync(SubscriptionDto subscription, IDbTransactionScope? scope = null);
        Task UpdateAsync(SubscriptionDto subscription, IDbTransactionScope? scope = null);
        Task DeleteAsync(Guid id, Guid userId, IDbTransactionScope? scope = null);
    }
}
