using System;

namespace SubscriptionTracker.Domain.Entities
{
    public sealed class HouseholdMembers
    {
        public Guid HouseholdId { get; set; }
        public Guid UserId { get; set; }
        public string Role { get; set; }
    }
}
