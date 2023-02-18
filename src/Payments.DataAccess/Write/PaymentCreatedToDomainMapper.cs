using EventStore.Client;
using Payments.Domain;

namespace Payments.DataAccess.Write;

public class PaymentCreatedToDomainMapper : SpecificEventToDomainMapper<PaymentCreated>
{
    public override bool CanMap(EventRecord evt) => evt.EventType == PaymentCreated.Type;
}