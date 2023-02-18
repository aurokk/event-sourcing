using Common.Abstractions;
using JetBrains.Annotations;

namespace Orders.Domain;

public enum OrderStatus
{
    Unknown = 0,
    Created = 1,
    PreparedToPay = 2,
    Paid = 3,
}

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class Order : Aggregate
{
    private OrderStatus Status = OrderStatus.Unknown;
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
        if (Status != OrderStatus.Created)
        {
            throw new InvalidOperationException($"Order must be in {nameof(OrderStatus.Created)} status");
        }

        var cartItemId = Guid.NewGuid().ToString("N");

        var @event = new ItemAddedToCart(
            aggregateId: Id!,
            productId: productId,
            cartItemId: cartItemId,
            createdAtUtc: utcNow
        );

        ApplyEvent(@event);
    }

    public void DeleteItemFromCart(string cartItemId, DateTime utcNow)
    {
        if (Status != OrderStatus.Created)
        {
            throw new InvalidOperationException($"Order must be in {nameof(OrderStatus.Created)} status");
        }

        if (_cart.All(x => x.Id != cartItemId))
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

    public async Task PrepareToPay(IPaymentsClient paymentsClient, DateTime utcNow, CancellationToken ct)
    {
        if (Status != OrderStatus.Created)
        {
            throw new InvalidOperationException($"Order must be in {nameof(OrderStatus.Created)} status");
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

        var @event = new OrderPreparedToPay(
            aggregateId: Id!,
            paymentId: createPaymentResponse.PaymentId,
            createdAtUtc: utcNow
        );

        ApplyEvent(@event);
    }

    public void SetPaid(DateTime utcNow)
    {
        if (Status != OrderStatus.PreparedToPay)
        {
            throw new InvalidOperationException($"Order must be in {nameof(OrderStatus.PreparedToPay)} status");
        }

        var @event = new OrderPaid(aggregateId: Id!, createdAtUtc: utcNow);

        ApplyEvent(@event);
    }

    protected void Apply(OrderCreated @event)
    {
        Id = @event.AggregateId;
        Status = OrderStatus.Created;
    }

    protected void Apply(ItemAddedToCart @event)
    {
        var cartItem = new CartItem(@event.CartItemId, @event.ProductId);
        _cart.Add(cartItem);
    }

    protected void Apply(ItemDeletedFromCart @event)
    {
        _cart.RemoveAll(x => x.Id == @event.CartItemId);
    }

    protected void Apply(OrderPreparedToPay @event)
    {
        PaymentId = @event.PaymentId;
        Status = OrderStatus.PreparedToPay;
    }

    protected void Apply(OrderPaid @event)
    {
        Status = OrderStatus.Paid;
    }
}