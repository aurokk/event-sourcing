namespace Payments.Domain;

public interface IPaymentsRepository
{
    Task<Payment> Get(string id, CancellationToken ct);
    Task Save(Payment order, CancellationToken ct);
}