using EventStore.Client;
using MassTransit;
using Payments.Contracts;
using Payments.DataAccess.Write;
using Payments.Domain;

namespace Payments.Api.Authorizer;

public class NotifierProcessor : IEventProcessor
{
    private readonly EventToDomainMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IPaymentsRepository _paymentsRepository;

    public NotifierProcessor(
        EventToDomainMapper mapper,
        IPublishEndpoint publishEndpoint,
        IPaymentsRepository paymentsRepository)
    {
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
        _paymentsRepository = paymentsRepository;
    }

    public bool CanProcess(EventRecord eventRecord) =>
        eventRecord.EventType == PaymentFulfilled.Type;

    public async Task Process(EventRecord eventRecord, CancellationToken ct)
    {
        if (_mapper.Map(eventRecord) is not PaymentFulfilled domainEvent)
        {
            throw new Exception();
        }

        var payment = await _paymentsRepository.Get(domainEvent.AggregateId, ct);
        var notification = new PaymentFulfilledNotification(payment.ReferenceId!, payment.Id!);
        await _publishEndpoint.Publish(notification, ct);
    }
}