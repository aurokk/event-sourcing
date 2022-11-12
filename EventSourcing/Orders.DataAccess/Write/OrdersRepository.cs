using EventStore.Client;
using Orders.Domain;

namespace Orders.DataAccess.Write;

public class OrdersRepository : IOrdersRepository
{
    private readonly EventStoreClient _client;
    private readonly EventToDomainMapper _toDomainMapper;

    public OrdersRepository(EventStoreClient client, EventToDomainMapper toDomainMapper)
    {
        _client = client;
        _toDomainMapper = toDomainMapper;
    }

    public async Task<Order> Get(string id, CancellationToken ct)
    {
        var result = _client.ReadStreamAsync(
            Direction.Forwards,
            id,
            StreamPosition.Start,
            cancellationToken: ct);

        var eventsDto = await result.ToListAsync(ct);

        var events = eventsDto
            .Select(x => x.Event)
            .Select(_toDomainMapper.Map)
            .ToArray();

        var order = new Order();
        order.LoadFromHistory(events);
        return order;
    }

    public async Task Save(Order order, CancellationToken ct)
    {
        if (order.Id == null)
        {
            throw new Exception();
        }

        var eventsDto = order
            .GetEventsToCommit()
            .Select(EventToDtoMapper.Map)
            .ToArray();

        await _client.AppendToStreamAsync(
            order.Id,
            StreamState.Any,
            eventsDto,
            cancellationToken: ct
        );
    }
}