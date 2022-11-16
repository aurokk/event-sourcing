using Common.Abstractions;

namespace Payments.Domain;

public class PaymentCreated : DomainEvent
{
    public const string Type = "PaymentCreated";
    public override string EventType => Type;

    public PaymentCreated(string aggregateId, DateTime createdAtUtc)
        : base(aggregateId, createdAtUtc)
    {
    }
}