using EventStore.Client;
using Orders.DataAccess.Write;
using Orders.Domain;
using IOrdersRepository = Orders.DataAccess.Read.IOrdersRepository;

namespace Orders.Api.OrdersProjections;

public class ItemAddedToCartProcessor : IEventProcessor
{
    private readonly EventToDomainMapper _mapper;
    private readonly IOrdersRepository _ordersRepository;

    public ItemAddedToCartProcessor(EventToDomainMapper mapper, IOrdersRepository ordersRepository)
    {
        _mapper = mapper;
        _ordersRepository = ordersRepository;
    }

    public bool CanProcess(EventRecord eventRecord) =>
        eventRecord.EventType == ItemAddedToCart.Type;

    public async Task Process(EventRecord eventRecord, CancellationToken ct)
    {
        if (_mapper.Map(eventRecord) is not ItemAddedToCart domainEvent)
        {
            throw new Exception();
        }

        var order = await _ordersRepository.Get(domainEvent.AggregateId, ct);
        order.AddItemToCart(productId: domainEvent.ProductId, cartItemId: domainEvent.CartItemId);
        await _ordersRepository.Update(order, ct);
    }
}