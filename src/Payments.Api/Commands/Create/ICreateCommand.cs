namespace Payments.Api.Commands.Create;

public interface ICreateCommand
{
    Task<CreateCommandResult> Execute(
        string referenceId,
        decimal amount,
        int currencyCode,
        CancellationToken ct);
}