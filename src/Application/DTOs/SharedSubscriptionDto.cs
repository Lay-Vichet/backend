using System;

namespace SubscriptionTracker.Application.DTOs;

public sealed record SharedSubscriptionDto
{
    public Guid SubscriptionId { get; init; }
    public Guid HouseholdId { get; init; }
}
