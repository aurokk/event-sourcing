using System.Net.Mime;
using System.Text;
using JetBrains.Annotations;
using LightBDD.Framework.Scenarios;
using LightBDD.NUnit3;
using Newtonsoft.Json;

namespace Orders.E2eTests;

[UsedImplicitly]
public class Tests_000Context
{
    private readonly HttpClient _httpClient;

    private static readonly Uri _ordersBaseUri = new Uri("http://localhost:5142");
    private static readonly Uri _ordersCreateOrderUri = new Uri(_ordersBaseUri, "/orders/create");
    private static readonly Uri _ordersAddToCartUri = new Uri(_ordersBaseUri, "/orders/addtocart");
    private static readonly Uri _ordersDeleteFromCartUri = new Uri(_ordersBaseUri, "/orders/deletefromcart");
    private static readonly Uri _ordersCheckoutUri = new Uri(_ordersBaseUri, "/orders/checkout");
    private static readonly Uri _ordersGetUri = new Uri(_ordersBaseUri, "/orders");

    private static readonly Uri _paymentsBaseUri = new Uri("http://localhost:5249");

    public Tests_000Context()
    {
        _httpClient = new HttpClient();
    }

    private string? _orderId;

    public async Task CreateOrder()
    {
        var request = new HttpRequestMessage
        {
            Content = CreateJsonContent(new { }),
            Method = HttpMethod.Post,
            RequestUri = _ordersCreateOrderUri,
        };
        var response = await _httpClient.SendAsync(request);
        var result = await ParseResponse<CreateOrderResponse>(response);
        Assert.That(result.OrderId, Is.Not.Null);
        _orderId = result.OrderId;
    }

    public async Task AddItemToCart()
    {
        var message = new HttpRequestMessage
        {
            Content = CreateJsonContent(new
            {
                OrderId = _orderId,
                ProductId = Guid.NewGuid().ToString("N"),
            }),
            Method = HttpMethod.Post,
            RequestUri = _ordersAddToCartUri,
        };
        await _httpClient.SendAsync(message);
    }

    public async Task DeleteItemFromCart()
    {
        var request = new HttpRequestMessage
        {
            Content = CreateJsonContent(new
            {
                OrderId = _orderId,
                CartItemId = "",
            }),
            Method = HttpMethod.Post,
            RequestUri = _ordersDeleteFromCartUri,
        };
        var response = await _httpClient.SendAsync(request);
    }

    public async Task Checkout()
    {
        var message = new HttpRequestMessage
        {
            Content = CreateJsonContent(new { }),
            Method = HttpMethod.Post,
            RequestUri = _ordersCheckoutUri,
        };
        await _httpClient.SendAsync(message);
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

[UsedImplicitly]
public class CreateOrderResponse
{
    public string? OrderId { get; set; }
}

public class Tests_000 : FeatureFixture
{
    [Scenario]
    public async Task Test_000()
    {
        await Runner
            .WithContext<Tests_000Context>()
            .RunScenarioAsync(
                c => c.CreateOrder(),
                c => c.AddItemToCart()
            );
    }
}