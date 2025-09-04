using System;

namespace SubscriptionTracker.Domain.Entities
{
    public sealed class PaymentMethods
    {
        public Guid PaymentMethodId { get; set; }
        public Guid UserId { get; set; }
        public string CardBrand { get; set; }
        public string LastFour { get; set; }
        public string DisplayName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
