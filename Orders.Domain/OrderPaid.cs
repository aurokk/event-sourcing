using Common.Abstractions;

namespace Orders.Domain;

public class OrderPaid : DomainEvent
{
    public const string Type = "OrderPaid";
    public override string EventType => Type;

    public OrderPaid(string aggregateId, DateTime createdAtUtc) : base(aggregateId, createdAtUtc)
    {
    }
}