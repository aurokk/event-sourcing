namespace Payments.Api.Commands.Authorize;

public interface IAuthorizeCommand
{
    Task<AuthorizeCommandResponse> Execute(string paymentId, CancellationToken ct);
}