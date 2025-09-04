using System;

namespace SubscriptionTracker.Domain.Entities
{
    public sealed class Subscriptions
    {
        public Guid SubscriptionId { get; set; }
        public Guid UserId { get; set; }
        public required string Name { get; set; }
        public required decimal Cost { get; set; }
        public required string Currency { get; set; }
        public required string BillingCycle { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime? NextDueDate { get; set; }
        public required string Notes { get; set; }
        public required bool? IsTrialPeriod { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}
