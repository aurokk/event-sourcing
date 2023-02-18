using EventStore.Client;
using Payments.DataAccess.Write;
using Payments.Domain;
using IPaymentsRepository = Payments.DataAccess.Read.IPaymentsRepository;

namespace Payments.Api.PaymentsProjections;

public class PaymentStartedProcessor : IEventProcessor
{
    private readonly EventToDomainMapper _mapper;
    private readonly IPaymentsRepository _paymentsRepository;

    public PaymentStartedProcessor(EventToDomainMapper mapper, IPaymentsRepository paymentsRepository)
    {
        _mapper = mapper;
        _paymentsRepository = paymentsRepository;
    }

    public bool CanProcess(EventRecord eventRecord) =>
        eventRecord.EventType == PaymentStarted.Type;

    public async Task Process(EventRecord eventRecord, CancellationToken ct)
    {
        if (_mapper.Map(eventRecord) is not PaymentStarted domainEvent)
        {
            throw new Exception();
        }

        var payment = await _paymentsRepository.Get(domainEvent.AggregateId, ct);
        payment.SetStarted();
        await _paymentsRepository.Update(payment, ct);
    }
}