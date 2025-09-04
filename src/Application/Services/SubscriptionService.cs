using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOfWork _uow;

        public SubscriptionService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SubscriptionDto> CreateAsync(SubscriptionDto dto)
        {
            var toCreate = dto with { Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id };
            await _uow.Subscriptions.AddAsync(toCreate);
            return toCreate;
        }

        public async Task DeleteAsync(Guid id)
        {
            await _uow.Subscriptions.DeleteAsync(id);
        }

        public async Task<IEnumerable<SubscriptionDto>> GetAllAsync()
        {
            return await _uow.Subscriptions.GetAllAsync();
        }

        public async Task<SubscriptionDto?> GetByIdAsync(Guid id)
        {
            return await _uow.Subscriptions.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Guid id, SubscriptionDto dto)
        {
            var existing = await _uow.Subscriptions.GetByIdAsync(id);
            if (existing is null) throw new KeyNotFoundException("Subscription not found");

            var updated = dto with { Id = id };
            await _uow.Subscriptions.UpdateAsync(updated);
        }
    }
}
