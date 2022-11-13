using EventStore.Client;
using Orders.Domain;

namespace Orders.DataAccess.Write;

public class OrderPlacedToDomainMapper : SpecificEventToDomainMapper<OrderPlaced>
{
    public override bool CanMap(EventRecord evt) => evt.EventType == OrderPlaced.Type;
}