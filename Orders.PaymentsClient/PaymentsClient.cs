using Orders.Domain;

namespace Orders.PaymentsClient;

public class PaymentsClient : IPaymentsClient
{
    public Task<CreatePaymentResponse> CreatePayment(CreatePaymentRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}