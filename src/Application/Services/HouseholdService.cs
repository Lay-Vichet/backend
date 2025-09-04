using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Application.Services
{
    public class HouseholdService : IHouseholdService
    {
        private readonly IUnitOfWork _uow;

        public HouseholdService(IUnitOfWork uow) => _uow = uow;

        public Task<IEnumerable<HouseholdDto>> GetAllAsync() => _uow.Households.GetAllAsync();
        public Task<HouseholdDto?> GetByIdAsync(Guid id) => _uow.Households.GetByIdAsync(id);
        public Task AddAsync(HouseholdDto dto) => _uow.Households.AddAsync(dto);
        public Task UpdateAsync(HouseholdDto dto) => _uow.Households.UpdateAsync(dto);
        public Task DeleteAsync(Guid id) => _uow.Households.DeleteAsync(id);
    }
}
