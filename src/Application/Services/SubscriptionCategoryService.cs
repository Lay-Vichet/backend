using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Application.Services
{
    public class SubscriptionCategoryService : ISubscriptionCategoryService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public SubscriptionCategoryService(IUnitOfWorkFactory uowFactory) => _uowFactory = uowFactory;

        public async Task<IEnumerable<SubscriptionCategoryDto>> GetAllAsync()
        {
            await using var uow = _uowFactory.Create();
            return await uow.SubscriptionCategories.GetAllAsync();
        }

        public async Task<SubscriptionCategoryDto?> GetByIdAsync(Guid subscriptionId, Guid categoryId)
        {
            await using var uow = _uowFactory.Create();
            return await uow.SubscriptionCategories.GetByIdAsync(subscriptionId, categoryId);
        }

        public async Task AddAsync(SubscriptionCategoryDto dto)
        {
            await using var uow = _uowFactory.Create();
            await uow.SubscriptionCategories.AddAsync(dto);
            await uow.CommitAsync();
        }

        public async Task UpdateAsync(SubscriptionCategoryDto dto)
        {
            await using var uow = _uowFactory.Create();
            await uow.SubscriptionCategories.UpdateAsync(dto);
            await uow.CommitAsync();
        }

        public async Task DeleteAsync(Guid subscriptionId, Guid categoryId)
        {
            await using var uow = _uowFactory.Create();
            await uow.SubscriptionCategories.DeleteAsync(subscriptionId, categoryId);
            await uow.CommitAsync();
        }
    }
}
