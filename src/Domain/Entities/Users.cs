using System;

namespace SubscriptionTracker.Domain.Entities
{
    public sealed class Users
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
