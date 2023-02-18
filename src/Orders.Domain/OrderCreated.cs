using Common.Abstractions;

namespace Orders.Domain;

public class OrderCreated : DomainEvent
{
    public const string Type = "OrderCreated";
    public override string EventType => Type;

    public OrderCreated(string aggregateId, DateTime createdAtUtc)
        : base(aggregateId, createdAtUtc)
    {
    }
}