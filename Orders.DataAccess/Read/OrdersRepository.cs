using Raven.Client.Documents;

namespace Orders.DataAccess.Read;

public class OrdersRepository : IOrdersRepository
{
    private readonly IDocumentStore _store;

    public OrdersRepository(IDocumentStore store) => _store = store;

    public Task Create(Order order, CancellationToken ct) =>
        Store(order, ct);

    public Task Update(Order order, CancellationToken ct) =>
        Store(order, ct);

    private async Task Store(Order order, CancellationToken ct)
    {
        using var session = _store.OpenAsyncSession();
        var orderDto = ToDto(order);
        await session.StoreAsync(
            entity: orderDto,
            id: orderDto.Id,
            changeVector: order.ChangeVector,
            token: ct
        );
        var changeVector = session.Advanced.GetChangeVectorFor(orderDto);
        order.Commit(changeVector);
        await session.SaveChangesAsync(ct);
    }

    public async Task<Order> Get(string id, CancellationToken ct)
    {
        using var session = _store.OpenAsyncSession();
        var orderDto = await session.LoadAsync<OrderDto>(id, ct);
        if (orderDto == null)
        {
            throw new Exception();
        }

        var changeVector = session.Advanced.GetChangeVectorFor(orderDto);
        return ToDomain(orderDto, changeVector);
    }

    private static Order ToDomain(OrderDto orderDto, string changeVector) =>
        Order.FromDatabase(
            id: orderDto.Id ?? throw new Exception(),
            changeVector: changeVector,
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
        };
}