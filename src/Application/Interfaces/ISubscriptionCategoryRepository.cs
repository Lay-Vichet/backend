using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;

namespace SubscriptionTracker.Application.Interfaces
{
    public interface ISubscriptionCategoryRepository
    {
        Task<IEnumerable<SubscriptionCategoryDto>> GetAllAsync();
        Task<SubscriptionCategoryDto?> GetByIdAsync(Guid subscriptionId, Guid categoryId);
        Task AddAsync(SubscriptionCategoryDto dto, IDbTransactionScope? scope = null);
        Task UpdateAsync(SubscriptionCategoryDto dto, IDbTransactionScope? scope = null);
        Task DeleteAsync(Guid subscriptionId, Guid categoryId, IDbTransactionScope? scope = null);
    }
}
