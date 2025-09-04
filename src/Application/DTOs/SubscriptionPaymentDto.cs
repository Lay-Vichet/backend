using System;

namespace SubscriptionTracker.Application.DTOs;

public sealed record SubscriptionPaymentDto
{
    public Guid PaymentId { get; init; }
    public Guid SubscriptionId { get; init; }
    public Guid? PaymentMethodId { get; init; }
    public decimal Amount { get; init; }
    public DateTime PaymentDate { get; init; }
    public DateTime CreatedAt { get; init; }
}
