using Payments.Domain;

namespace Payments.Api.Commands.Authorize;

public record AuthorizeCommandResponse(string PaymentId);

public class AuthorizeCommand : IAuthorizeCommand
{
    private readonly IPaymentsRepository _paymentsRepository;

    public AuthorizeCommand(IPaymentsRepository paymentsRepository) =>
        _paymentsRepository = paymentsRepository;

    public async Task<AuthorizeCommandResponse> Execute(string paymentId, CancellationToken ct)
    {
        var payment = await _paymentsRepository.Get(paymentId, ct);
        payment.Start(DateTime.UtcNow);
        await _paymentsRepository.Save(payment, ct);
        return new AuthorizeCommandResponse(paymentId);
    }
}