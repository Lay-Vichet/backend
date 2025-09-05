using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Application.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public PaymentMethodService(IUnitOfWorkFactory uowFactory) => _uowFactory = uowFactory;

        public async Task<IEnumerable<PaymentMethodDto>> GetAllAsync()
        {
            await using var uow = _uowFactory.Create();
            return await uow.PaymentMethods.GetAllAsync();
        }

        public async Task<PaymentMethodDto?> GetByIdAsync(Guid id)
        {
            await using var uow = _uowFactory.Create();
            return await uow.PaymentMethods.GetByIdAsync(id);
        }

        public async Task AddAsync(PaymentMethodDto dto)
        {
            await using var uow = _uowFactory.Create();
            await uow.PaymentMethods.AddAsync(dto);
            await uow.CommitAsync();
        }

        public async Task UpdateAsync(PaymentMethodDto dto)
        {
            await using var uow = _uowFactory.Create();
            await uow.PaymentMethods.UpdateAsync(dto);
            await uow.CommitAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await using var uow = _uowFactory.Create();
            await uow.PaymentMethods.DeleteAsync(id);
            await uow.CommitAsync();
        }
    }
}
