using Common.Abstractions;

namespace Orders.Domain;

public class OrderPreparedToPay : DomainEvent
{
    public const string Type = "OrderPreparedToPay";
    public override string EventType => Type;
    public string PaymentId { get; }

    public OrderPreparedToPay(string aggregateId, string paymentId, DateTime createdAtUtc) : base(aggregateId, createdAtUtc)
    {
        PaymentId = paymentId;
    }
}