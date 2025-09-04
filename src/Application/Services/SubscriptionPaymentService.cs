using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Application.Services
{
    public class SubscriptionPaymentService : ISubscriptionPaymentService
    {
        private readonly IUnitOfWork _uow;

        public SubscriptionPaymentService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<IEnumerable<SubscriptionPaymentDto>> GetAllAsync() => _uow.SubscriptionPayments.GetAllAsync();
        public Task<SubscriptionPaymentDto?> GetByIdAsync(Guid id) => _uow.SubscriptionPayments.GetByIdAsync(id);
        public Task AddAsync(SubscriptionPaymentDto dto) => _uow.SubscriptionPayments.AddAsync(dto);
        public Task UpdateAsync(SubscriptionPaymentDto dto) => _uow.SubscriptionPayments.UpdateAsync(dto);
        public Task DeleteAsync(Guid id) => _uow.SubscriptionPayments.DeleteAsync(id);

        public async Task AddPaymentAndUpdateSubscriptionAsync(SubscriptionPaymentDto payment, SubscriptionTracker.Application.DTOs.SubscriptionDto updatedSubscription)
        {
            try
            {
                await _uow.SubscriptionPayments.AddAsync(payment);
                await _uow.Subscriptions.UpdateAsync(updatedSubscription);

                await _uow.CommitAsync();
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }
    }
}
