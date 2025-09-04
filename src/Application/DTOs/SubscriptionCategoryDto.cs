using System;

namespace SubscriptionTracker.Application.DTOs;

public sealed record SubscriptionCategoryDto
{
    public Guid SubscriptionId { get; init; }
    public Guid CategoryId { get; init; }
}
