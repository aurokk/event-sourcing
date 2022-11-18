using Payments.Domain;

namespace Payments.Api.Commands.Create;

public record CreateCommandResult(string PaymentId);

public class CreateCommand : ICreateCommand
{
    private readonly IPaymentsRepository _paymentsRepository;

    public CreateCommand(IPaymentsRepository paymentsRepository)
    {
        _paymentsRepository = paymentsRepository;
    }

    public async Task<CreateCommandResult> Execute(
        string referenceId,
        decimal amount,
        int currencyCode,
        CancellationToken ct)
    {
        var paymentId = Guid.NewGuid().ToString("N");
        var payment = Payment.Create(paymentId, referenceId, amount, currencyCode, DateTime.UtcNow);
        await _paymentsRepository.Save(payment, ct);
        return new CreateCommandResult(paymentId);
    }
}