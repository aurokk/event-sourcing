using Common.Abstractions;

namespace Payments.Domain;

public class PaymentFulfilled : DomainEvent
{
    public const string Type = "PaymentFulfilled";
    public override string EventType => Type;

    public PaymentFulfilled(string aggregateId, DateTime createdAtUtc)
        : base(aggregateId, createdAtUtc)
    {
    }
}