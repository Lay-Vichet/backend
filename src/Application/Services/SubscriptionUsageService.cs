using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Application.Services
{
    public class SubscriptionUsageService : ISubscriptionUsageService
    {
        private readonly IUnitOfWork _uow;

        public SubscriptionUsageService(IUnitOfWork uow) => _uow = uow;

        public Task<IEnumerable<SubscriptionUsageDto>> GetAllAsync() => _uow.SubscriptionUsages.GetAllAsync();
        public Task<SubscriptionUsageDto?> GetByIdAsync(Guid id) => _uow.SubscriptionUsages.GetByIdAsync(id);
        public Task AddAsync(SubscriptionUsageDto dto) => _uow.SubscriptionUsages.AddAsync(dto);
        public Task UpdateAsync(SubscriptionUsageDto dto) => _uow.SubscriptionUsages.UpdateAsync(dto);
        public Task DeleteAsync(Guid id) => _uow.SubscriptionUsages.DeleteAsync(id);
    }
}
