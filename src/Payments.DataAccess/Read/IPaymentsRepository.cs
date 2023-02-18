namespace Payments.DataAccess.Read;

public interface IPaymentsRepository
{
    Task Create(Payment payment, CancellationToken ct);
    Task Update(Payment payment, CancellationToken ct);
    Task<Payment> Get(string id, CancellationToken ct);
}