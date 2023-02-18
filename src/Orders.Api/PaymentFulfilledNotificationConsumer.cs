using JetBrains.Annotations;
using MassTransit;
using Orders.Domain;
using Payments.Contracts;

namespace Orders.Api;

[UsedImplicitly]
public class PaymentFulfilledNotificationConsumer : IConsumer<PaymentCompletedNotification>
{
    private readonly IOrdersRepository _ordersRepository;

    public PaymentFulfilledNotificationConsumer(IOrdersRepository ordersRepository)
    {
        _ordersRepository = ordersRepository;
    }

    public async Task Consume(ConsumeContext<PaymentCompletedNotification> context)
    {
        var order = await _ordersRepository.Get(context.Message.ReferenceId, context.CancellationToken);
        order.SetPaid(DateTime.UtcNow);
        await _ordersRepository.Save(order, context.CancellationToken);
    }
}