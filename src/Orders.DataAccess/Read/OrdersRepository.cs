using MongoDB.Driver;

namespace Orders.DataAccess.Read;

public class OrdersRepository : IOrdersRepository
{
    private readonly IMongoCollection<OrderDto> _ordersCollection;

    public OrdersRepository(IMongoDatabase mongoDatabase) =>
        _ordersCollection = mongoDatabase.GetCollection<OrderDto>("orders");

    public Task Create(Order order, CancellationToken ct) =>
        Store(order, ct);

    public Task Update(Order order, CancellationToken ct) =>
        Store(order, ct);

    private async Task Store(Order order, CancellationToken ct)
    {
        var orderDto = ToDto(order);
        await _ordersCollection
            .ReplaceOneAsync(
                x => x.Id == order.Id && x.Etag == order.SavedEtag,
                orderDto,
                new ReplaceOptions { IsUpsert = true, },
                cancellationToken: ct
            );
        order.Commit();
    }

    public async Task<Order> Get(string id, CancellationToken ct)
    {
        var orderDto = await _ordersCollection
            .Find(x => x.Id == id)
            .SingleOrDefaultAsync(cancellationToken: ct);
        return orderDto != null
            ? ToDomain(orderDto)
            : throw new Exception();
    }

    private static Order ToDomain(OrderDto orderDto) =>
        Order.FromDatabase(
            id: orderDto.Id ?? throw new Exception(),
            etag: orderDto.Etag ?? throw new Exception(),
            orderStatus: Enum.Parse<OrderStatus>(orderDto.OrderStatus ?? throw new Exception()),
            cartItems: orderDto.Items?
                           .Select(x => new CartItem(
                                   Id: x.Id ?? throw new Exception(),
                                   ProductId: x.ProductId ?? throw new Exception()
                               )
                           )
                           .ToList() ??
                       new List<CartItem>(),
            paymentId: orderDto.PaymentId
        );

    private static OrderDto ToDto(Order order) =>
        new OrderDto
        {
            Id = order.Id,
            OrderStatus = order.OrderStatus.ToString("G"),
            Items = order.Cart
                .Select(x => new CartItemDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                })
                .ToList(),
            PaymentId = order.PaymentId,
            Etag = order.Etag,
        };
}