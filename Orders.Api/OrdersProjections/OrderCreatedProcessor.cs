using EventStore.Client;
using Orders.DataAccess.Write;
using Orders.Domain;
using IOrdersRepository = Orders.DataAccess.Read.IOrdersRepository;

namespace Orders.Api.OrdersProjections;

public class OrderCreatedProcessor : IEventProcessor
{
    private readonly EventToDomainMapper _mapper;
    private readonly IOrdersRepository _ordersRepository;

    public OrderCreatedProcessor(EventToDomainMapper mapper, IOrdersRepository ordersRepository)
    {
        _mapper = mapper;
        _ordersRepository = ordersRepository;
    }

    public bool CanProcess(EventRecord eventRecord) =>
        eventRecord.EventType == OrderCreated.Type;

    public async Task Process(EventRecord eventRecord, CancellationToken ct)
    {
        var domainEvent = _mapper.Map(eventRecord);
        var order = DataAccess.Read.Order.Create(domainEvent.AggregateId);
        await _ordersRepository.Create(order, ct);
    }
}