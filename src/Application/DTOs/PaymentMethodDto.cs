using System;

namespace SubscriptionTracker.Application.DTOs;

public sealed record PaymentMethodDto
{
    public Guid PaymentMethodId { get; init; }
    public Guid UserId { get; init; }
    public string CardBrand { get; init; } = string.Empty;
    public string LastFour { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
