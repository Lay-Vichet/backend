using System;

namespace SubscriptionTracker.Application.DTOs;

public sealed record CategoryDto
{
    public Guid CategoryId { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
