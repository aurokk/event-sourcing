using System.Net.Mime;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Orders.Domain;

namespace Orders.PaymentsClient;

public record CreatePaymentRequestDto(string ReferenceId, decimal Amount, int CurrencyCode);

[PublicAPI]
public record CreatePaymentResponseDto(string PaymentId);

public class PaymentsClient : IPaymentsClient
{
    private readonly HttpClient _httpClient;

    private static readonly Uri _paymentsBaseUri =
        new Uri(Environment.GetEnvironmentVariable("PaymentsApi__Host") ?? throw new AggregateException());

    private static readonly Uri _paymentsCreateUri = new Uri(_paymentsBaseUri, "/payments/create");

    public PaymentsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CreatePaymentResponse> CreatePayment(CreatePaymentRequest request, CancellationToken ct)
    {
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            Content = CreateJsonContent(
                new CreatePaymentRequestDto(
                    request.ReferenceId,
                    request.Amount,
                    request.CurrencyCode)),
            RequestUri = _paymentsCreateUri,
        };
        var responseMessage = await _httpClient.SendAsync(requestMessage, ct);
        var result = await ParseResponse<CreatePaymentResponseDto>(responseMessage);
        return new CreatePaymentResponse(result.PaymentId);
    }

    private static HttpContent CreateJsonContent(object data)
    {
        var body = JsonConvert.SerializeObject(data);
        return new StringContent(body, Encoding.UTF8, MediaTypeNames.Application.Json);
    }

    private static async Task<T> ParseResponse<T>(HttpResponseMessage responseMessage)
    {
        responseMessage.EnsureSuccessStatusCode();
        var data = await responseMessage.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<T>(data);
        return result ?? throw new Exception();
    }
}