using EventStore.Client;
using Payments.DataAccess.Write;
using Payments.Domain;
using IPaymentsRepository = Payments.DataAccess.Read.IPaymentsRepository;

namespace Payments.Api.PaymentsProjections;

public class PaymentCreatedProcessor : IEventProcessor
{
    private readonly EventToDomainMapper _mapper;
    private readonly IPaymentsRepository _paymentsRepository;

    public PaymentCreatedProcessor(EventToDomainMapper mapper, IPaymentsRepository paymentsRepository)
    {
        _mapper = mapper;
        _paymentsRepository = paymentsRepository;
    }

    public bool CanProcess(EventRecord eventRecord) =>
        eventRecord.EventType == PaymentCreated.Type;

    public async Task Process(EventRecord eventRecord, CancellationToken ct)
    {
        var domainEvent = _mapper.Map(eventRecord);
        var payment = DataAccess.Read.Payment.Create(domainEvent.AggregateId);
        await _paymentsRepository.Create(payment, ct);
    }
}