using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Application.Services
{
    public class SharedSubscriptionService : ISharedSubscriptionService
    {
        private readonly IUnitOfWork _uow;

        public SharedSubscriptionService(IUnitOfWork uow) => _uow = uow;

        public Task<IEnumerable<SharedSubscriptionDto>> GetAllAsync() => _uow.SharedSubscriptions.GetAllAsync();
        public Task<SharedSubscriptionDto?> GetByIdAsync(Guid subscriptionId, Guid householdId) => _uow.SharedSubscriptions.GetByIdAsync(subscriptionId, householdId);
        public Task AddAsync(SharedSubscriptionDto dto) => _uow.SharedSubscriptions.AddAsync(dto);
        public Task UpdateAsync(SharedSubscriptionDto dto) => _uow.SharedSubscriptions.UpdateAsync(dto);
        public Task DeleteAsync(Guid subscriptionId, Guid householdId) => _uow.SharedSubscriptions.DeleteAsync(subscriptionId, householdId);
    }
}
