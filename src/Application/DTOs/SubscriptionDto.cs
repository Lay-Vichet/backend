using System;

namespace SubscriptionTracker.Application.DTOs;

public sealed record SubscriptionDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal MonthlyCost { get; init; }
    public DateTime StartDate { get; init; }
}
