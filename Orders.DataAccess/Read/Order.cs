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
    public string? ChangeVector { get; private set; }
    private readonly List<CartItem> _cart;
    public CartItem[] Cart => _cart.ToArray();

    private Order(string id, string? changeVector, OrderStatus orderStatus, List<CartItem> cartItems)
    {
        Id = id;
        ChangeVector = changeVector;
        OrderStatus = orderStatus;
        _cart = cartItems;
    }

    public static Order Create(string id) =>
        new Order(
            id: id,
            changeVector: null,
            orderStatus: OrderStatus.Initialized,
            cartItems: new List<CartItem>()
        );

    public static Order FromDatabase(
        string id,
        string changeVector,
        OrderStatus orderStatus,
        List<CartItem> cartItems
    ) =>
        new Order(
            id: id,
            changeVector: changeVector,
            orderStatus: orderStatus,
            cartItems: cartItems
        );

    public void AddItemToCart(string productId, string cartItemId)
    {
        _cart.Add(new CartItem(Id: cartItemId, ProductId: productId));
    }

    public void DeleteItemFromCart(string cartItemId)
    {
        _cart.RemoveAll(x => x.Id == cartItemId);
    }

    public void Checkout()
    {
        OrderStatus = OrderStatus.Placed;
    }

    public void Commit(string changeVector)
    {
        ChangeVector = changeVector;
    }
}