using System;

namespace SubscriptionTracker.Application.DTOs;

public sealed record HouseholdDto
{
    public Guid HouseholdId { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid CreatedByUserId { get; init; }
    public DateTime CreatedAt { get; init; }
}
