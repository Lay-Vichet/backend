using System;

namespace SubscriptionTracker.Domain.Entities
{
    public sealed class Households
    {
        public Guid HouseholdId { get; set; }
        public string Name { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
