using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;

namespace SubscriptionTracker.Application.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(Guid id);
        Task AddAsync(CategoryDto dto, IDbTransactionScope? scope = null);
        Task UpdateAsync(CategoryDto dto, IDbTransactionScope? scope = null);
        Task DeleteAsync(Guid id, IDbTransactionScope? scope = null);
    }
}
