using EventStore.Client;
using Orders.DataAccess.Write;
using Orders.Domain;
using IOrdersRepository = Orders.DataAccess.Read.IOrdersRepository;

namespace Orders.Api.OrdersProjections;

public class ItemDeletedFromCartProcessor : IEventProcessor
{
    private readonly EventToDomainMapper _mapper;
    private readonly IOrdersRepository _ordersRepository;

    public ItemDeletedFromCartProcessor(EventToDomainMapper mapper, IOrdersRepository ordersRepository)
    {
        _mapper = mapper;
        _ordersRepository = ordersRepository;
    }

    public bool CanProcess(EventRecord eventRecord) =>
        eventRecord.EventType == ItemDeletedFromCart.Type;

    public async Task Process(EventRecord eventRecord, CancellationToken ct)
    {
        if (_mapper.Map(eventRecord) is not ItemDeletedFromCart domainEvent)
        {
            throw new Exception();
        }

        var order = await _ordersRepository.Get(domainEvent.AggregateId, ct);
        order.DeleteItemFromCart(domainEvent.CartItemId);
        await _ordersRepository.Update(order, ct);
    }
}