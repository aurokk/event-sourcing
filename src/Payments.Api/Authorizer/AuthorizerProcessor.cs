using EventStore.Client;
using Payments.DataAccess.Write;
using Payments.Domain;

namespace Payments.Api.Authorizer;

public class AuthorizerProcessor : IEventProcessor
{
    private readonly EventToDomainMapper _mapper;
    private readonly IPaymentsRepository _paymentsRepository;

    public AuthorizerProcessor(EventToDomainMapper mapper, IPaymentsRepository paymentsRepository)
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

        await Task.Delay(1000, ct);
        var payment = await _paymentsRepository.Get(domainEvent.AggregateId, ct);
        payment.Completed(DateTime.UtcNow);
        await _paymentsRepository.Save(payment, ct);
    }
}