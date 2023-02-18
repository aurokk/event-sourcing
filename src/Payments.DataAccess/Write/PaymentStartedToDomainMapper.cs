using EventStore.Client;
using Payments.Domain;

namespace Payments.DataAccess.Write;

public class PaymentStartedToDomainMapper : SpecificEventToDomainMapper<PaymentStarted>
{
    public override bool CanMap(EventRecord evt) => evt.EventType == PaymentStarted.Type;
}