using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _uow;

        public CategoryService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<IEnumerable<CategoryDto>> GetAllAsync() => _uow.Categories.GetAllAsync();
        public Task<CategoryDto?> GetByIdAsync(Guid id) => _uow.Categories.GetByIdAsync(id);
        public Task AddAsync(CategoryDto dto) => _uow.Categories.AddAsync(dto);
        public Task UpdateAsync(CategoryDto dto) => _uow.Categories.UpdateAsync(dto);
        public Task DeleteAsync(Guid id) => _uow.Categories.DeleteAsync(id);
    }
}
