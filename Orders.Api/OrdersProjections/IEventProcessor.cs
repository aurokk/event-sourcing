using EventStore.Client;

namespace Orders.Api.OrdersProjections;

public interface IEventProcessor
{
    bool CanProcess(EventRecord eventRecord);
    Task Process(EventRecord eventRecord, CancellationToken ct);
}