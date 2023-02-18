using Orders.Domain;

namespace Orders.Api.Commands.Checkout;

public record CheckoutCommandRequest(string OrderId);

public record CheckoutCommandResponse;

public class CheckoutCommand : ICheckoutCommand
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IPaymentsClient _paymentsClient;

    public CheckoutCommand(IOrdersRepository ordersRepository, IPaymentsClient paymentsClient)
    {
        _ordersRepository = ordersRepository;
        _paymentsClient = paymentsClient;
    }

    public async Task<CheckoutCommandResponse> Execute(CheckoutCommandRequest request, CancellationToken ct)
    {
        var order = await _ordersRepository.Get(request.OrderId, ct);
        await order.PrepareToPay(_paymentsClient, DateTime.UtcNow, ct);
        await _ordersRepository.Save(order, ct);
        return new CheckoutCommandResponse();
    }
}