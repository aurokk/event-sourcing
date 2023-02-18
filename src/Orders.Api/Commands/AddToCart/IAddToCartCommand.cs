namespace Orders.Api.Commands.AddToCart;

public interface IAddToCartCommand
{
    Task<AddToCartCommandResponse> Execute(AddToCartCommandRequest request, CancellationToken ct);
}