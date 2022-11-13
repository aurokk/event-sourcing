using EventStore.Client;
using Orders.Domain;

namespace Orders.DataAccess.Write;

public class OrderPaidToDomainMapper : SpecificEventToDomainMapper<OrderPaid>
{
    public override bool CanMap(EventRecord evt) => evt.EventType == OrderPaid.Type;
}