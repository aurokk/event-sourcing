namespace Payments.Api.Commands.Create;

public interface ICreateCommand
{
    Task<CreateCommandResult> Execute(CancellationToken ct);
}