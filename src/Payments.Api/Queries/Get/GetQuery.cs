using Payments.DataAccess.Read;

namespace Payments.Api.Queries.Get;

public record GetResponse(Payment Payment);

public record Payment(string Id, string PaymentStatus);

public class GetQuery : IGetQuery
{
    private readonly IPaymentsRepository _paymentsRepository;

    public GetQuery(IPaymentsRepository paymentsRepository) =>
        _paymentsRepository = paymentsRepository;

    public async Task<GetResponse> Execute(string PaymentId, CancellationToken ct)
    {
        var order = await _paymentsRepository.Get(PaymentId, ct);
        return new GetResponse(
            Payment: new Payment(
                Id: order.Id,
                PaymentStatus: order.PaymentStatus.ToString("G")
            )
        );
    }
}