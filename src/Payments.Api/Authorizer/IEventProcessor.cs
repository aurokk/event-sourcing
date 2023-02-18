using EventStore.Client;

namespace Payments.Api.Authorizer;

public interface IEventProcessor
{
    bool CanProcess(EventRecord eventRecord);
    Task Process(EventRecord eventRecord, CancellationToken ct);
}