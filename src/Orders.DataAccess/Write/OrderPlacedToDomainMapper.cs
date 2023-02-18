using EventStore.Client;
using Orders.Domain;

namespace Orders.DataAccess.Write;

public class OrderPlacedToDomainMapper : SpecificEventToDomainMapper<OrderPreparedToPay>
{
    public override bool CanMap(EventRecord evt) => evt.EventType == OrderPreparedToPay.Type;
}