namespace Orders.Api.Commands.DeleteFromCart;

public interface IDeleteFromCartCommand
{
    Task<DeleteFromCartCommandResponse> Execute(DeleteFromCartCommandRequest request, CancellationToken ct);
}