using Common.Abstractions;

namespace Orders.Domain;

public class ItemAddedToCart : DomainEvent
{
    public const string Type = "ItemAddedToCart";
    public override string EventType => Type;
    public string ProductId { get; }
    public string CartItemId { get; }

    public ItemAddedToCart(string aggregateId, string productId, string cartItemId, DateTime createdAtUtc)
        : base(aggregateId, createdAtUtc)
    {
        ProductId = productId;
        CartItemId = cartItemId;
    }
}