using System;
using System.Text.Json.Serialization;

namespace SubscriptionTracker.Application.DTOs;

public sealed record SubscriptionDto
{
    public Guid Id { get; init; }
    [JsonIgnore]
    public Guid UserId { get; set; }
    public required string Name { get; init; }
    public decimal Cost { get; init; }
    public required string Currency { get; set; }
    public required string BillingCycle { get; set; }
    public DateTime StartDate { get; init; }
    public DateTime? NextDueDate { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
}
