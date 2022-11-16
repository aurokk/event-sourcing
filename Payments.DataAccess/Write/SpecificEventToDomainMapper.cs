using System.Text.Json;
using Common.Abstractions;
using EventStore.Client;

namespace Payments.DataAccess.Write;

public abstract class SpecificEventToDomainMapper<T> : ISpecificEventToDomainMapper
    where T : DomainEvent
{
    public abstract bool CanMap(EventRecord evt);

    public DomainEvent Map(EventRecord evt) =>
        JsonSerializer.Deserialize<T>(evt.Data.Span) ?? throw new Exception();
}