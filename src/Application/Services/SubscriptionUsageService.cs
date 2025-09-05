using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Application.Services
{
    public class SubscriptionUsageService : ISubscriptionUsageService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public SubscriptionUsageService(IUnitOfWorkFactory uowFactory) => _uowFactory = uowFactory;

        public async Task<IEnumerable<SubscriptionUsageDto>> GetAllAsync()
        {
            await using var uow = _uowFactory.Create();
            return await uow.SubscriptionUsages.GetAllAsync();
        }

        public async Task<SubscriptionUsageDto?> GetByIdAsync(Guid id)
        {
            await using var uow = _uowFactory.Create();
            return await uow.SubscriptionUsages.GetByIdAsync(id);
        }

        public async Task AddAsync(SubscriptionUsageDto dto)
        {
            await using var uow = _uowFactory.Create();
            await uow.SubscriptionUsages.AddAsync(dto);
            await uow.CommitAsync();
        }

        public async Task UpdateAsync(SubscriptionUsageDto dto)
        {
            await using var uow = _uowFactory.Create();
            await uow.SubscriptionUsages.UpdateAsync(dto);
            await uow.CommitAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await using var uow = _uowFactory.Create();
            await uow.SubscriptionUsages.DeleteAsync(id);
            await uow.CommitAsync();
        }
    }
}
