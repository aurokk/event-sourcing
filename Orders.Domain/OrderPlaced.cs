using Common.Abstractions;

namespace Orders.Domain;

public class OrderPlaced : DomainEvent
{
    public const string Type = "OrderPlaced";
    public override string EventType => Type;
    public string PaymentId { get; }

    public OrderPlaced(string aggregateId, string paymentId, DateTime createdAtUtc) : base(aggregateId, createdAtUtc)
    {
        PaymentId = paymentId;
    }
}