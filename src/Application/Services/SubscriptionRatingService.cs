using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Application.Services
{
    public class SubscriptionRatingService : ISubscriptionRatingService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public SubscriptionRatingService(IUnitOfWorkFactory uowFactory) => _uowFactory = uowFactory;

        public async Task<IEnumerable<SubscriptionRatingDto>> GetAllAsync()
        {
            await using var uow = _uowFactory.Create();
            return await uow.SubscriptionRatings.GetAllAsync();
        }

        public async Task<SubscriptionRatingDto?> GetByIdAsync(Guid id)
        {
            await using var uow = _uowFactory.Create();
            return await uow.SubscriptionRatings.GetByIdAsync(id);
        }

        public async Task AddAsync(SubscriptionRatingDto dto)
        {
            await using var uow = _uowFactory.Create();
            await uow.SubscriptionRatings.AddAsync(dto);
            await uow.CommitAsync();
        }

        public async Task UpdateAsync(SubscriptionRatingDto dto)
        {
            await using var uow = _uowFactory.Create();
            await uow.SubscriptionRatings.UpdateAsync(dto);
            await uow.CommitAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await using var uow = _uowFactory.Create();
            await uow.SubscriptionRatings.DeleteAsync(id);
            await uow.CommitAsync();
        }
    }
}
