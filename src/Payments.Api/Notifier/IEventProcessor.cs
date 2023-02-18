using EventStore.Client;

namespace Payments.Api.Notifier;

public interface IEventProcessor
{
    bool CanProcess(EventRecord eventRecord);
    Task Process(EventRecord eventRecord, CancellationToken ct);
}