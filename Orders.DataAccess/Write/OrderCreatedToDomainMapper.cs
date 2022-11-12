using EventStore.Client;
using Orders.Domain;

namespace Orders.DataAccess.Write;

public class OrderCreatedToDomainMapper : SpecificEventToDomainMapper<OrderCreated>
{
    public override bool CanMap(EventRecord evt) => evt.EventType == OrderCreated.Type;
}