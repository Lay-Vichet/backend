using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;

namespace SubscriptionTracker.Application.Interfaces
{
    public interface ISubscriptionUsageRepository
    {
        Task<IEnumerable<SubscriptionUsageDto>> GetAllAsync();
        Task<SubscriptionUsageDto?> GetByIdAsync(Guid id);
        Task AddAsync(SubscriptionUsageDto dto, IDbTransactionScope? scope = null);
        Task UpdateAsync(SubscriptionUsageDto dto, IDbTransactionScope? scope = null);
        Task DeleteAsync(Guid id, IDbTransactionScope? scope = null);
    }
}
