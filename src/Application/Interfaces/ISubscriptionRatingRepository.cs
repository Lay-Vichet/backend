using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;

namespace SubscriptionTracker.Application.Interfaces
{
    public interface ISubscriptionRatingRepository
    {
        Task<IEnumerable<SubscriptionRatingDto>> GetAllAsync();
        Task<SubscriptionRatingDto?> GetByIdAsync(Guid id);
        Task AddAsync(SubscriptionRatingDto dto, IDbTransactionScope? scope = null);
        Task UpdateAsync(SubscriptionRatingDto dto, IDbTransactionScope? scope = null);
        Task DeleteAsync(Guid id, IDbTransactionScope? scope = null);
    }
}
