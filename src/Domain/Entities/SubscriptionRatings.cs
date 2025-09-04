using System;

namespace SubscriptionTracker.Domain.Entities
{
    public sealed class SubscriptionRatings
    {
        public Guid RatingId { get; set; }
        public Guid SubscriptionId { get; set; }
        public Guid UserId { get; set; }
        public int Score { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
