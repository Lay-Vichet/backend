using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Application.Services
{
    public class SubscriptionPaymentService : ISubscriptionPaymentService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public SubscriptionPaymentService(IUnitOfWorkFactory uowFactory)
        {
            _uowFactory = uowFactory;
        }

        public Task<IEnumerable<SubscriptionPaymentDto>> GetAllAsync()
        {
            var uow = _uowFactory.Create();
            return uow.SubscriptionPayments.GetAllAsync();
        }

        public async Task<SubscriptionPaymentDto?> GetByIdAsync(Guid id)
        {
            await using var uow = _uowFactory.Create();
            return await uow.SubscriptionPayments.GetByIdAsync(id);
        }

        public async Task AddAsync(SubscriptionPaymentDto dto)
        {
            await using var uow = _uowFactory.Create();
            await uow.SubscriptionPayments.AddAsync(dto);
            await uow.CommitAsync();
        }

        public async Task UpdateAsync(SubscriptionPaymentDto dto)
        {
            await using var uow = _uowFactory.Create();
            await uow.SubscriptionPayments.UpdateAsync(dto);
            await uow.CommitAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await using var uow = _uowFactory.Create();
            await uow.SubscriptionPayments.DeleteAsync(id);
            await uow.CommitAsync();
        }

        public async Task AddPaymentAndUpdateSubscriptionAsync(SubscriptionPaymentDto payment, SubscriptionDto updatedSubscription)
        {
            await using var uow = _uowFactory.Create();
            try
            {
                await uow.SubscriptionPayments.AddAsync(payment);
                await uow.Subscriptions.UpdateAsync(updatedSubscription);

                await uow.CommitAsync();
            }
            catch
            {
                await uow.RollbackAsync();
                throw;
            }
        }
    }
}
