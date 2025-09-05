using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public UserService(IUnitOfWorkFactory uowFactory) => _uowFactory = uowFactory;

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            await using var uow = _uowFactory.Create();
            return await uow.Users.GetAllAsync();
        }

        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            await using var uow = _uowFactory.Create();
            return await uow.Users.GetByIdAsync(id);
        }

        public async Task AddAsync(UserDto dto)
        {
            await using var uow = _uowFactory.Create();
            await uow.Users.AddAsync(dto);
            await uow.CommitAsync();
        }

        public async Task UpdateAsync(UserDto dto)
        {
            await using var uow = _uowFactory.Create();
            await uow.Users.UpdateAsync(dto);
            await uow.CommitAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await using var uow = _uowFactory.Create();
            await uow.Users.DeleteAsync(id);
            await uow.CommitAsync();
        }
    }
}
