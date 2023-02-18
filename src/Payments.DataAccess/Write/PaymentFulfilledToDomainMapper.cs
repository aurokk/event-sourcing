using EventStore.Client;
using Payments.Domain;

namespace Payments.DataAccess.Write;

public class PaymentFulfilledToDomainMapper : SpecificEventToDomainMapper<PaymentCompleted>
{
    public override bool CanMap(EventRecord evt) => evt.EventType == PaymentCompleted.Type;
}