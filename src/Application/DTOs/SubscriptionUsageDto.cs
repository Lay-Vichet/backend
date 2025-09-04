using System;

namespace SubscriptionTracker.Application.DTOs;

public sealed record SubscriptionUsageDto
{
    public Guid UsageId { get; init; }
    public Guid SubscriptionId { get; init; }
    public Guid UserId { get; init; }
    public DateTime LoggedAt { get; init; }
}
