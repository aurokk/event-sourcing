using EventStore.Client;
using Payments.Domain;

namespace Payments.DataAccess.Write;

public class PaymentsRepository : IPaymentsRepository
{
    private readonly EventStoreClient _client;
    private readonly EventToDomainMapper _toDomainMapper;

    public PaymentsRepository(EventStoreClient client, EventToDomainMapper toDomainMapper)
    {
        _client = client;
        _toDomainMapper = toDomainMapper;
    }

    public async Task<Payment> Get(string id, CancellationToken ct)
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

        var payment = new Payment();
        payment.LoadFromHistory(events);
        return payment;
    }

    public async Task Save(Payment order, CancellationToken ct)
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