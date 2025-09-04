using System;

namespace SubscriptionTracker.Application.DTOs;

public sealed record SubscriptionRatingDto
{
    public Guid RatingId { get; init; }
    public Guid SubscriptionId { get; init; }
    public Guid UserId { get; init; }
    public int Score { get; init; }
    public DateTime CreatedAt { get; init; }
}
