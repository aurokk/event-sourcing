using EventStore.Client;
using Orders.DataAccess.Write;
using Orders.Domain;
using IOrdersRepository = Orders.DataAccess.Read.IOrdersRepository;

namespace Orders.Api.OrdersProjections;

public class OrderPlacedProcessor : IEventProcessor
{
    private readonly EventToDomainMapper _mapper;
    private readonly IOrdersRepository _ordersRepository;

    public OrderPlacedProcessor(EventToDomainMapper mapper, IOrdersRepository ordersRepository)
    {
        _mapper = mapper;
        _ordersRepository = ordersRepository;
    }

    public bool CanProcess(EventRecord eventRecord) =>
        eventRecord.EventType == OrderPreparedToPay.Type;

    public async Task Process(EventRecord eventRecord, CancellationToken ct)
    {
        if (_mapper.Map(eventRecord) is not OrderPreparedToPay domainEvent)
        {
            throw new Exception();
        }

        var order = await _ordersRepository.Get(domainEvent.AggregateId, ct);
        order.Checkout(domainEvent.PaymentId);
        await _ordersRepository.Update(order, ct);
    }
}