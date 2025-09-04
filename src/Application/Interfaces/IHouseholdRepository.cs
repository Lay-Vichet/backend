using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;

namespace SubscriptionTracker.Application.Interfaces
{
    public interface IHouseholdRepository
    {
        Task<IEnumerable<HouseholdDto>> GetAllAsync();
        Task<HouseholdDto?> GetByIdAsync(Guid id);
        Task AddAsync(HouseholdDto dto, IDbTransactionScope? scope = null);
        Task UpdateAsync(HouseholdDto dto, IDbTransactionScope? scope = null);
        Task DeleteAsync(Guid id, IDbTransactionScope? scope = null);
    }
}
