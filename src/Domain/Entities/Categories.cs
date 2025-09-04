using System;

namespace SubscriptionTracker.Domain.Entities
{
    public sealed class Categories
    {
        public Guid CategoryId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
