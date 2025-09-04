using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;

        public UserService(IUnitOfWork uow) => _uow = uow;

        public Task<IEnumerable<UserDto>> GetAllAsync() => _uow.Users.GetAllAsync();
        public Task<UserDto?> GetByIdAsync(Guid id) => _uow.Users.GetByIdAsync(id);
        public Task AddAsync(UserDto dto) => _uow.Users.AddAsync(dto);
        public Task UpdateAsync(UserDto dto) => _uow.Users.UpdateAsync(dto);
        public Task DeleteAsync(Guid id) => _uow.Users.DeleteAsync(id);
    }
}
