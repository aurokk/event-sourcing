namespace Orders.Api.Commands.Create;

public record CreateCommandResponse(string OrderId);

public interface ICreateCommand
{
    Task<CreateCommandResponse> Execute(CancellationToken ct);
}