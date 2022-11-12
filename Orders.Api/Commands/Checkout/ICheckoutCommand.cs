namespace Orders.Api.Commands.Checkout;

public interface ICheckoutCommand
{
    Task<CheckoutCommandResponse> Execute(CheckoutCommandRequest request, CancellationToken ct);
}