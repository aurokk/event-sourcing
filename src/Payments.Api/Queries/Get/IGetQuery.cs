namespace Payments.Api.Queries.Get;

public interface IGetQuery
{
    Task<GetResponse> Execute(string PaymentId, CancellationToken ct);
}