using Common.Abstractions;
using EventStore.Client;

namespace Orders.DataAccess.Write;

public interface ISpecificEventToDomainMapper
{
    bool CanMap(EventRecord evt);
    DomainEvent Map(EventRecord evt);
}