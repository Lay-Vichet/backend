using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Application.Services
{
    public class SubscriptionCategoryService : ISubscriptionCategoryService
    {
        private readonly IUnitOfWork _uow;

        public SubscriptionCategoryService(IUnitOfWork uow) => _uow = uow;

        public Task<IEnumerable<SubscriptionCategoryDto>> GetAllAsync() => _uow.SubscriptionCategories.GetAllAsync();
        public Task<SubscriptionCategoryDto?> GetByIdAsync(Guid subscriptionId, Guid categoryId) => _uow.SubscriptionCategories.GetByIdAsync(subscriptionId, categoryId);
        public Task AddAsync(SubscriptionCategoryDto dto) => _uow.SubscriptionCategories.AddAsync(dto);
        public Task UpdateAsync(SubscriptionCategoryDto dto) => _uow.SubscriptionCategories.UpdateAsync(dto);
        public Task DeleteAsync(Guid subscriptionId, Guid categoryId) => _uow.SubscriptionCategories.DeleteAsync(subscriptionId, categoryId);
    }
}
