using Common.Abstractions;

namespace Payments.Domain;

public class PaymentCompleted : DomainEvent
{
    public const string Type = "PaymentCompleted";
    public override string EventType => Type;

    public PaymentCompleted(string aggregateId, DateTime createdAtUtc)
        : base(aggregateId, createdAtUtc)
    {
    }
}