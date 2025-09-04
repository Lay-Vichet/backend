using System;

namespace SubscriptionTracker.Domain.Entities
{
    public sealed class SubscriptionCategories
    {
        public Guid SubscriptionId { get; set; }
        public Guid CategoryId { get; set; }
    }
}
