namespace Orders.Domain;

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