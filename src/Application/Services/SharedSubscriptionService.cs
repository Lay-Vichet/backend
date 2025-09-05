using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Application.Services
{
    public class SharedSubscriptionService : ISharedSubscriptionService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public SharedSubscriptionService(IUnitOfWorkFactory uowFactory) => _uowFactory = uowFactory;

        public async Task<IEnumerable<SharedSubscriptionDto>> GetAllAsync()
        {
            await using var uow = _uowFactory.Create();
            return await uow.SharedSubscriptions.GetAllAsync();
        }

        public async Task<SharedSubscriptionDto?> GetByIdAsync(Guid subscriptionId, Guid householdId)
        {
            await using var uow = _uowFactory.Create();
            return await uow.SharedSubscriptions.GetByIdAsync(subscriptionId, householdId);
        }

        public async Task AddAsync(SharedSubscriptionDto dto)
        {
            await using var uow = _uowFactory.Create();
            await uow.SharedSubscriptions.AddAsync(dto);
            await uow.CommitAsync();
        }

        public async Task UpdateAsync(SharedSubscriptionDto dto)
        {
            await using var uow = _uowFactory.Create();
            await uow.SharedSubscriptions.UpdateAsync(dto);
            await uow.CommitAsync();
        }

        public async Task DeleteAsync(Guid subscriptionId, Guid householdId)
        {
            await using var uow = _uowFactory.Create();
            await uow.SharedSubscriptions.DeleteAsync(subscriptionId, householdId);
            await uow.CommitAsync();
        }
    }
}
