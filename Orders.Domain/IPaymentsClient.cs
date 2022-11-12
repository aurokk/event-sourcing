namespace Orders.Domain;

public interface IPaymentsClient
{
    Task<CreatePaymentResponse> CreatePayment(CreatePaymentRequest request, CancellationToken ct);
}