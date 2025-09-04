using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;

namespace SubscriptionTracker.Application.Interfaces
{
    public interface IHouseholdMemberRepository
    {
        Task<IEnumerable<HouseholdMemberDto>> GetAllAsync();
        Task<HouseholdMemberDto?> GetByIdAsync(Guid householdId, Guid userId);
        Task AddAsync(HouseholdMemberDto dto, IDbTransactionScope? scope = null);
        Task UpdateAsync(HouseholdMemberDto dto, IDbTransactionScope? scope = null);
        Task DeleteAsync(Guid householdId, Guid userId, IDbTransactionScope? scope = null);
    }
}
