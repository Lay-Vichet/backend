using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public SubscriptionService(IUnitOfWorkFactory uowFactory)
        {
            _uowFactory = uowFactory;
        }

        public async Task<SubscriptionDto> CreateAsync(SubscriptionDto dto)
        {
            var toCreate = dto with { Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id };
            await using var uow = _uowFactory.Create();
            await uow.Subscriptions.AddAsync(toCreate);
            await uow.CommitAsync();
            return toCreate;
        }

        public async Task DeleteAsync(Guid id)
        {
            await using var uow = _uowFactory.Create();
            await uow.Subscriptions.DeleteAsync(id);
            await uow.CommitAsync();
        }

        public async Task<IEnumerable<SubscriptionDto>> GetAllAsync()
        {
            await using var uow = _uowFactory.Create();
            return await uow.Subscriptions.GetAllAsync();
        }

        public async Task<SubscriptionDto?> GetByIdAsync(Guid id)
        {
            await using var uow = _uowFactory.Create();
            return await uow.Subscriptions.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Guid id, SubscriptionDto dto)
        {
            await using var uow = _uowFactory.Create();
            var existing = await uow.Subscriptions.GetByIdAsync(id);
            if (existing is null) throw new KeyNotFoundException("Subscription not found");
            var updated = dto with { Id = id };
            await uow.Subscriptions.UpdateAsync(updated);
            await uow.CommitAsync();
        }
    }
}
