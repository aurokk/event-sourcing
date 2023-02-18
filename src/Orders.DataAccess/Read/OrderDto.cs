namespace Orders.DataAccess.Read;

public class OrderDto
{
    public string? Id { get; set; }
    public string? OrderStatus { get; set; }
    public List<CartItemDto>? Items { get; set; }
    public string? PaymentId { get; set; }
    public string? Etag { get; set; }
}