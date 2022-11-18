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
    public string? ChangeVector { get; private set; }

    private Order(string id, string? changeVector, OrderStatus orderStatus, List<CartItem> cartItems, string? paymentId)
    {
        Id = id;
        ChangeVector = changeVector;
        OrderStatus = orderStatus;
        _cart = cartItems;
        PaymentId = paymentId;
    }

    public static Order Create(string id) =>
        new Order(
            id: id,
            changeVector: null,
            orderStatus: OrderStatus.Initialized,
            cartItems: new List<CartItem>(),
            paymentId: null
        );

    public static Order FromDatabase(
        string id,
        string changeVector,
        OrderStatus orderStatus,
        List<CartItem> cartItems,
        string? paymentId
    ) =>
        new Order(
            id: id,
            changeVector: changeVector,
            orderStatus: orderStatus,
            cartItems: cartItems,
            paymentId: paymentId
        );

    public void AddItemToCart(string productId, string cartItemId)
    {
        _cart.Add(new CartItem(Id: cartItemId, ProductId: productId));
    }

    public void DeleteItemFromCart(string cartItemId)
    {
        _cart.RemoveAll(x => x.Id == cartItemId);
    }

    public void Checkout(string paymentId)
    {
        PaymentId = paymentId;
        OrderStatus = OrderStatus.Placed;
    }

    public void Paid()
    {
        OrderStatus = OrderStatus.Paid;
    }

    public void Commit(string changeVector)
    {
        ChangeVector = changeVector;
    }
}