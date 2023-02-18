namespace Orders.DataAccess.Read;

public enum OrderStatus
{
    Initialized = 1,
    Placed = 2,
    Paid = 3,
}

public record CartItem(string Id, string ProductId);

public class Order
{
    public string Id { get; }
    public OrderStatus OrderStatus { get; private set; }
    private readonly List<CartItem> _cart;
    public CartItem[] Cart => _cart.ToArray();

    public string? PaymentId { get; private set; }
    public string? Etag { get; private set; }
    public string? SavedEtag { get; private set; }

    private Order(
        string id,
        string etag,
        string? savedEtag,
        OrderStatus orderStatus,
        List<CartItem> cartItems,
        string? paymentId)
    {
        Id = id;
        Etag = etag;
        SavedEtag = savedEtag;
        OrderStatus = orderStatus;
        _cart = cartItems;
        PaymentId = paymentId;
    }

    public static Order Create(string id) =>
        new Order(
            id: id,
            etag: Guid.NewGuid().ToString("N"),
            savedEtag: null,
            orderStatus: OrderStatus.Initialized,
            cartItems: new List<CartItem>(),
            paymentId: null
        );

    public static Order FromDatabase(
        string id,
        string etag,
        OrderStatus orderStatus,
        List<CartItem> cartItems,
        string? paymentId
    ) =>
        new Order(
            id: id,
            etag: etag,
            savedEtag: etag,
            orderStatus: orderStatus,
            cartItems: cartItems,
            paymentId: paymentId
        );

    public void AddItemToCart(string productId, string cartItemId)
    {
        _cart.Add(new CartItem(Id: cartItemId, ProductId: productId));
        Etag = Guid.NewGuid().ToString("N");
    }

    public void DeleteItemFromCart(string cartItemId)
    {
        _cart.RemoveAll(x => x.Id == cartItemId);
        Etag = Guid.NewGuid().ToString("N");
    }

    public void Checkout(string paymentId)
    {
        PaymentId = paymentId;
        OrderStatus = OrderStatus.Placed;
        Etag = Guid.NewGuid().ToString("N");
    }

    public void Paid()
    {
        OrderStatus = OrderStatus.Paid;
        Etag = Guid.NewGuid().ToString("N");
    }

    public void Commit()
    {
        SavedEtag = Etag;
    }
}