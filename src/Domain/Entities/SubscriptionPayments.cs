using System;

namespace SubscriptionTracker.Domain.Entities
{
    public sealed class SubscriptionPayments
    {
        public Guid PaymentId { get; set; }
        public Guid SubscriptionId { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
