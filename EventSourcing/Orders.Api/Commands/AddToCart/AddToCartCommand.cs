using Orders.Domain;

namespace Orders.Api.Commands.AddToCart;

public record AddToCartCommandRequest(string OrderId, string ProductId);

public record AddToCartCommandResponse;

public class AddToCartCommand : IAddToCartCommand
{
    private readonly IOrdersRepository _ordersRepository;

    public AddToCartCommand(IOrdersRepository ordersRepository)
    {
        _ordersRepository = ordersRepository;
    }

    public async Task<AddToCartCommandResponse> Execute(AddToCartCommandRequest request, CancellationToken ct)
    {
        var order = await _ordersRepository.Get(request.OrderId, ct);
        var utcNow = DateTime.UtcNow;
        order.AddItemToCart(request.ProductId, utcNow);
        await _ordersRepository.Save(order, ct);
        return new AddToCartCommandResponse();
    }
}