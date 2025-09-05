using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Application.Services
{
    public class HouseholdMemberService : IHouseholdMemberService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public HouseholdMemberService(IUnitOfWorkFactory uowFactory) => _uowFactory = uowFactory;

        public async Task<IEnumerable<HouseholdMemberDto>> GetAllAsync()
        {
            await using var uow = _uowFactory.Create();
            return await uow.HouseholdMembers.GetAllAsync();
        }

        public async Task<HouseholdMemberDto?> GetByIdAsync(Guid householdId, Guid userId)
        {
            await using var uow = _uowFactory.Create();
            return await uow.HouseholdMembers.GetByIdAsync(householdId, userId);
        }

        public async Task AddAsync(HouseholdMemberDto dto)
        {
            await using var uow = _uowFactory.Create();
            await uow.HouseholdMembers.AddAsync(dto);
            await uow.CommitAsync();
        }

        public async Task UpdateAsync(HouseholdMemberDto dto)
        {
            await using var uow = _uowFactory.Create();
            await uow.HouseholdMembers.UpdateAsync(dto);
            await uow.CommitAsync();
        }

        public async Task DeleteAsync(Guid householdId, Guid userId)
        {
            await using var uow = _uowFactory.Create();
            await uow.HouseholdMembers.DeleteAsync(householdId, userId);
            await uow.CommitAsync();
        }
    }
}
