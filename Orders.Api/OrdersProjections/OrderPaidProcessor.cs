using EventStore.Client;
using Orders.DataAccess.Write;
using Orders.Domain;
using IOrdersRepository = Orders.DataAccess.Read.IOrdersRepository;

namespace Orders.Api.OrdersProjections;

public class OrderPaidProcessor : IEventProcessor
{
    private readonly EventToDomainMapper _mapper;
    private readonly IOrdersRepository _ordersRepository;

    public OrderPaidProcessor(EventToDomainMapper mapper, IOrdersRepository ordersRepository)
    {
        _mapper = mapper;
        _ordersRepository = ordersRepository;
    }

    public bool CanProcess(EventRecord eventRecord) =>
        eventRecord.EventType == OrderPaid.Type;

    public Task Process(EventRecord eventRecord, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}