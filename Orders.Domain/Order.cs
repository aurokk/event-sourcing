using Common.Abstractions;
using JetBrains.Annotations;

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

public class ItemAddedToCart : DomainEvent
{
    public const string Type = "ItemAddedToCart";
    public override string EventType => Type;
    public string ProductId { get; }

    public ItemAddedToCart(string aggregateId, string productId, DateTime createdAtUtc)
        : base(aggregateId, createdAtUtc)
    {
        ProductId = productId;
    }
}

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

public class OrderPaid : DomainEvent
{
    public const string Type = "OrderPaid";
    public override string EventType => Type;

    public OrderPaid(string aggregateId, DateTime createdAtUtc) : base(aggregateId, createdAtUtc)
    {
    }
}

public class CartItem
{
    public string Id { get; }
    public string ProductId { get; }

    public CartItem(string id, string productId)
    {
        Id = id;
        ProductId = productId;
    }
}

public enum OrderStatus
{
    Uninitialized = 0,
    Initialized = 1,
    Placed = 2,
    Paid = 3,
}

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class Order : Aggregate
{
    private OrderStatus Status = OrderStatus.Uninitialized;
    private readonly List<CartItem> _cart = new();
    public CartItem[] Cart => _cart.ToArray();
    private string? PaymentId;

    public Order()
    {
    }

    public static Order Create(string id, DateTime utcNow)
    {
        var order = new Order();
        var @event = new OrderCreated(
            aggregateId: id,
            createdAtUtc: utcNow
        );
        order.ApplyEvent(@event);
        return order;
    }

    public void AddItemToCart(string productId, DateTime utcNow)
    {
        if (Status != OrderStatus.Initialized)
        {
            throw new InvalidOperationException($"Order must be in {nameof(OrderStatus.Initialized)} status");
        }

        var @event = new ItemAddedToCart(
            aggregateId: Id!,
            productId: productId,
            createdAtUtc: utcNow
        );

        ApplyEvent(@event);
    }

    public void DeleteItemFromCart(string cartItemId, DateTime utcNow)
    {
        if (Status != OrderStatus.Initialized)
        {
            throw new InvalidOperationException($"Order must be in {nameof(OrderStatus.Initialized)} status");
        }

        if (_cart.All(x => x.ProductId != cartItemId))
        {
            throw new InvalidOperationException($"Order must contain at least one item with product id {cartItemId}");
        }

        var @event = new ItemDeletedFromCart(
            aggregateId: Id!,
            cartItemId: cartItemId,
            createdAtUtc: utcNow
        );

        ApplyEvent(@event);
    }

    public async Task Place(IPaymentsClient paymentsClient, DateTime utcNow, CancellationToken ct)
    {
        if (Status != OrderStatus.Initialized)
        {
            throw new InvalidOperationException($"Order must be in {nameof(OrderStatus.Initialized)} status");
        }

        if (!_cart.Any())
        {
            throw new InvalidOperationException($"Order must contain at least one item");
        }

        var createPaymentRequest = new CreatePaymentRequest(
            ReferenceId: Id!,
            Amount: 100,
            CurrencyCode: 840
        );

        var createPaymentResponse = await paymentsClient.CreatePayment(createPaymentRequest, ct);

        var @event = new OrderPlaced(
            aggregateId: Id!,
            paymentId: createPaymentResponse.PaymentId,
            createdAtUtc: utcNow
        );

        ApplyEvent(@event);
    }

    public void Paid(DateTime utcNow)
    {
        if (Status != OrderStatus.Placed)
        {
            throw new InvalidOperationException($"Order must be in {nameof(OrderStatus.Placed)} status");
        }

        var @event = new OrderPaid(aggregateId: Id!, createdAtUtc: utcNow);

        ApplyEvent(@event);
    }

    protected void Apply(OrderCreated @event)
    {
        Id = @event.AggregateId;
        Status = OrderStatus.Initialized;
    }

    protected void Apply(ItemAddedToCart @event)
    {
        var cartItemId = Guid.NewGuid().ToString("N");
        var cartItem = new CartItem(cartItemId, @event.ProductId);
        _cart.Add(cartItem);
    }

    protected void Apply(ItemDeletedFromCart @event)
    {
        _cart.RemoveAll(x => x.Id == @event.CartItemId);
    }

    protected void Apply(OrderPlaced @event)
    {
        PaymentId = @event.PaymentId;
        Status = OrderStatus.Placed;
    }

    protected void Apply(OrderPaid @event)
    {
        Status = OrderStatus.Paid;
    }
}