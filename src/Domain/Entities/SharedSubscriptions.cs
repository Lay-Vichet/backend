using System;

namespace SubscriptionTracker.Domain.Entities
{
    public sealed class SharedSubscriptions
    {
        public Guid SubscriptionId { get; set; }
        public Guid HouseholdId { get; set; }
    }
}
