using EventStore.Client;
using Payments.Domain;

namespace Payments.DataAccess.Write;

public class PaymentFulfilledToDomainMapper : SpecificEventToDomainMapper<PaymentFulfilled>
{
    public override bool CanMap(EventRecord evt) => evt.EventType == PaymentFulfilled.Type;
}