using Orders.Domain;

namespace Orders.Api.Commands.Create;

public class CreateCommand : ICreateCommand
{
    private readonly IOrdersRepository _ordersRepository;

    public CreateCommand(IOrdersRepository ordersRepository)
    {
        _ordersRepository = ordersRepository;
    }

    public async Task<CreateCommandResponse> Execute(CancellationToken ct)
    {
        var orderId = Guid.NewGuid().ToString("N");
        var datetime = DateTime.UtcNow;
        var order = Order.Create(orderId, datetime);
        await _ordersRepository.Save(order, ct);
        return new CreateCommandResponse(OrderId: orderId);
    }
}