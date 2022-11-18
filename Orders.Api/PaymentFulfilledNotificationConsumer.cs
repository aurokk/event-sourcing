using MassTransit;
using Orders.Domain;
using Payments.Contracts;

namespace Orders.Api;

public class PaymentFulfilledNotificationConsumer : IConsumer<PaymentFulfilledNotification>
{
    private readonly IOrdersRepository _ordersRepository;

    public PaymentFulfilledNotificationConsumer(IOrdersRepository ordersRepository)
    {
        _ordersRepository = ordersRepository;
    }

    public async Task Consume(ConsumeContext<PaymentFulfilledNotification> context)
    {
        var order = await _ordersRepository.Get(context.Message.ReferenceId, context.CancellationToken);
        order.Paid(DateTime.UtcNow);
        await _ordersRepository.Save(order, context.CancellationToken);
    }
}