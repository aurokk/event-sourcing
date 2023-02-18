using Orders.Domain;

namespace Orders.Api.Commands.DeleteFromCart;

public record DeleteFromCartCommandRequest(string OrderId, string CartItemId);

public record DeleteFromCartCommandResponse;

public class DeleteFromCartCommand : IDeleteFromCartCommand
{
    private readonly IOrdersRepository _ordersRepository;

    public DeleteFromCartCommand(IOrdersRepository ordersRepository)
    {
        _ordersRepository = ordersRepository;
    }

    public async Task<DeleteFromCartCommandResponse> Execute(DeleteFromCartCommandRequest request, CancellationToken ct)
    {
        var order = await _ordersRepository.Get(request.OrderId, ct);
        var utcNow = DateTime.UtcNow;
        order.DeleteItemFromCart(request.CartItemId, utcNow);
        await _ordersRepository.Save(order, ct);
        return new DeleteFromCartCommandResponse();
    }
}