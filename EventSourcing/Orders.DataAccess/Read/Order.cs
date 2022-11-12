namespace Orders.DataAccess.Read;

public class Order
{
    public string Id { get; }

    private Order(string id)
    {
        Id = id;
    }

    public static Order Create(string id)
    {
        return new Order(
            id: id
        );
    }

    public static Order FromDatabase(string id)
    {
        return new Order(
            id: id
        );
    }

    public void AddItemToCart()
    {
    }

    public void DeleteItemFromCart()
    {
    }
}