using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Application.Services
{
    public class SubscriptionRatingService : ISubscriptionRatingService
    {
        private readonly IUnitOfWork _uow;

        public SubscriptionRatingService(IUnitOfWork uow) => _uow = uow;

        public Task<IEnumerable<SubscriptionRatingDto>> GetAllAsync() => _uow.SubscriptionRatings.GetAllAsync();
        public Task<SubscriptionRatingDto?> GetByIdAsync(Guid id) => _uow.SubscriptionRatings.GetByIdAsync(id);
        public Task AddAsync(SubscriptionRatingDto dto) => _uow.SubscriptionRatings.AddAsync(dto);
        public Task UpdateAsync(SubscriptionRatingDto dto) => _uow.SubscriptionRatings.UpdateAsync(dto);
        public Task DeleteAsync(Guid id) => _uow.SubscriptionRatings.DeleteAsync(id);
    }
}
