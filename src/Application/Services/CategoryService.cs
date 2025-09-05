using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public CategoryService(IUnitOfWorkFactory uowFactory)
        {
            _uowFactory = uowFactory;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            await using var uow = _uowFactory.Create();
            return await uow.Categories.GetAllAsync();
        }

        public async Task<CategoryDto?> GetByIdAsync(Guid id)
        {
            await using var uow = _uowFactory.Create();
            return await uow.Categories.GetByIdAsync(id);
        }

        public async Task AddAsync(CategoryDto dto)
        {
            await using var uow = _uowFactory.Create();
            await uow.Categories.AddAsync(dto);
            await uow.CommitAsync();
        }

        public async Task UpdateAsync(CategoryDto dto)
        {
            await using var uow = _uowFactory.Create();
            await uow.Categories.UpdateAsync(dto);
            await uow.CommitAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await using var uow = _uowFactory.Create();
            await uow.Categories.DeleteAsync(id);
            await uow.CommitAsync();
        }
    }
}
