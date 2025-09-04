using System;

namespace SubscriptionTracker.Domain.Entities
{
    public sealed class SubscriptionUsage
    {
        public Guid UsageId { get; set; }
        public Guid SubscriptionId { get; set; }
        public Guid UserId { get; set; }
        public DateTime LoggedAt { get; set; }
    }
}
