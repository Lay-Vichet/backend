using System;

namespace SubscriptionTracker.Application.DTOs;

public sealed record HouseholdMemberDto
{
    public Guid HouseholdId { get; init; }
    public Guid UserId { get; init; }
    public string Role { get; init; } = string.Empty;
}
