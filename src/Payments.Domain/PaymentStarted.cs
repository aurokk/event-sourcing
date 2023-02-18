using Common.Abstractions;

namespace Payments.Domain;

public class PaymentStarted : DomainEvent
{
    public const string Type = "PaymentStarted";
    public override string EventType => Type;

    public PaymentStarted(string aggregateId, DateTime createdAtUtc)
        : base(aggregateId, createdAtUtc)
    {
    }
}