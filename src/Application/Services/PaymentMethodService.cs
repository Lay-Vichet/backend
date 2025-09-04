using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Application.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IUnitOfWork _uow;

        public PaymentMethodService(IUnitOfWork uow) => _uow = uow;

        public Task<IEnumerable<PaymentMethodDto>> GetAllAsync() => _uow.PaymentMethods.GetAllAsync();
        public Task<PaymentMethodDto?> GetByIdAsync(Guid id) => _uow.PaymentMethods.GetByIdAsync(id);
        public Task AddAsync(PaymentMethodDto dto) => _uow.PaymentMethods.AddAsync(dto);
        public Task UpdateAsync(PaymentMethodDto dto) => _uow.PaymentMethods.UpdateAsync(dto);
        public Task DeleteAsync(Guid id) => _uow.PaymentMethods.DeleteAsync(id);
    }
}
