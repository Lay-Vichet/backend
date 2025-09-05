using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Application.Services
{
    public class HouseholdService : IHouseholdService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public HouseholdService(IUnitOfWorkFactory uowFactory) => _uowFactory = uowFactory;

        public async Task<IEnumerable<HouseholdDto>> GetAllAsync()
        {
            await using var uow = _uowFactory.Create();
            return await uow.Households.GetAllAsync();
        }

        public async Task<HouseholdDto?> GetByIdAsync(Guid id)
        {
            await using var uow = _uowFactory.Create();
            return await uow.Households.GetByIdAsync(id);
        }

        public async Task AddAsync(HouseholdDto dto)
        {
            await using var uow = _uowFactory.Create();
            await uow.Households.AddAsync(dto);
            await uow.CommitAsync();
        }

        public async Task UpdateAsync(HouseholdDto dto)
        {
            await using var uow = _uowFactory.Create();
            await uow.Households.UpdateAsync(dto);
            await uow.CommitAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await using var uow = _uowFactory.Create();
            await uow.Households.DeleteAsync(id);
            await uow.CommitAsync();
        }
    }
}
