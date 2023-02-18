using Common.Abstractions;

namespace Orders.Domain;

public class ItemDeletedFromCart : DomainEvent
{
    public const string Type = "ItemDeletedFromCart";
    public override string EventType => Type;
    public string CartItemId { get; }

    public ItemDeletedFromCart(string aggregateId, string cartItemId, DateTime createdAtUtc)
        : base(aggregateId, createdAtUtc)
    {
        CartItemId = cartItemId;
    }
}