using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Application.Services
{
    public class HouseholdMemberService : IHouseholdMemberService
    {
        private readonly IUnitOfWork _uow;

        public HouseholdMemberService(IUnitOfWork uow) => _uow = uow;

        public Task<IEnumerable<HouseholdMemberDto>> GetAllAsync() => _uow.HouseholdMembers.GetAllAsync();
        public Task<HouseholdMemberDto?> GetByIdAsync(Guid householdId, Guid userId) => _uow.HouseholdMembers.GetByIdAsync(householdId, userId);
        public Task AddAsync(HouseholdMemberDto dto) => _uow.HouseholdMembers.AddAsync(dto);
        public Task UpdateAsync(HouseholdMemberDto dto) => _uow.HouseholdMembers.UpdateAsync(dto);
        public Task DeleteAsync(Guid householdId, Guid userId) => _uow.HouseholdMembers.DeleteAsync(householdId, userId);
    }
}
