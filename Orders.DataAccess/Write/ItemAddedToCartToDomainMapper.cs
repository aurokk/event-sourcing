using EventStore.Client;
using Orders.Domain;

namespace Orders.DataAccess.Write;

public class ItemAddedToCartToDomainMapper : SpecificEventToDomainMapper<ItemAddedToCart>
{
    public override bool CanMap(EventRecord evt) => evt.EventType == ItemAddedToCart.Type;
}