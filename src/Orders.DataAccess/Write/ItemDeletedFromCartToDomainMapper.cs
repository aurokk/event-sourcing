using EventStore.Client;
using Orders.Domain;

namespace Orders.DataAccess.Write;

public class ItemDeletedFromCartToDomainMapper : SpecificEventToDomainMapper<ItemDeletedFromCart>
{
    public override bool CanMap(EventRecord evt) => evt.EventType == ItemDeletedFromCart.Type;
}