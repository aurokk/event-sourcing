using System.Net.Mime;
using System.Text;
using JetBrains.Annotations;
using LightBDD.Framework.Scenarios;
using LightBDD.NUnit3;
using Newtonsoft.Json;

namespace E2eTests;

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
    private static readonly Uri _paymentsAuthorizeUri = new Uri(_paymentsBaseUri, "/payments/authorize");

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
        var requestMessage = new HttpRequestMessage
        {
            Content = CreateJsonContent(new
            {
                OrderId = _orderId,
                ProductId = Guid.NewGuid().ToString("N"),
            }),
            Method = HttpMethod.Post,
            RequestUri = _ordersAddToCartUri,
        };
        var responseMessage = await _httpClient.SendAsync(requestMessage);
        responseMessage.EnsureSuccessStatusCode();
    }

    private OrderDto? _order;

    public async Task GetOrder()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_ordersGetUri, $"?orderId={_orderId}"),
        };
        var response = await _httpClient.SendAsync(request);
        var result = await ParseResponse<GetOrderResponse>(response);
        Assert.That(result.Order, Is.Not.Null);
        _order = result.Order;
    }

    public async Task DeleteItemFromCart()
    {
        var cartItemId = _order?.Cart?.FirstOrDefault()?.Id ?? throw new ApplicationException();

        var requestMessage = new HttpRequestMessage
        {
            Content = CreateJsonContent(new
            {
                OrderId = _orderId,
                CartItemId = cartItemId,
            }),
            Method = HttpMethod.Post,
            RequestUri = _ordersDeleteFromCartUri,
        };

        var responseMessage = await _httpClient.SendAsync(requestMessage);
        responseMessage.EnsureSuccessStatusCode();
    }

    public async Task Checkout()
    {
        Assert.That(_order?.Id, Is.Not.Null);

        var requestMessage = new HttpRequestMessage
        {
            Content = CreateJsonContent(new
            {
                OrderId = _order?.Id,
            }),
            Method = HttpMethod.Post,
            RequestUri = _ordersCheckoutUri,
        };
        var responseMessage = await _httpClient.SendAsync(requestMessage);
        responseMessage.EnsureSuccessStatusCode();
    }

    public async Task Authorize()
    {
        Assert.That(_order?.PaymentId, Is.Not.Null);

        var requestMessage = new HttpRequestMessage
        {
            Content = CreateJsonContent(new
            {
                PaymentId = _order?.PaymentId,
            }),
            Method = HttpMethod.Post,
            RequestUri = _paymentsAuthorizeUri,
        };
        var responseMessage = await _httpClient.SendAsync(requestMessage);
        responseMessage.EnsureSuccessStatusCode();
    }

    public Task Check()
    {
        Assert.That(_order?.OrderStatus, Is.EqualTo("Paid"));
        return Task.CompletedTask;
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

[PublicAPI]
public class CreateOrderResponse
{
    public string? OrderId { get; set; }
}

[PublicAPI]
public class CartItemDto
{
    public string? Id { get; set; }
    public string? ProductId { get; set; }
}

[PublicAPI]
public class OrderDto
{
    public string? Id { get; set; }
    public CartItemDto[]? Cart { get; set; }
    public string? PaymentId { get; set; }
    public string? OrderStatus { get; set; }
}

[PublicAPI]
public class GetOrderResponse
{
    public OrderDto? Order { get; set; }
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
                c => c.AddItemToCart(),
                c => c.AddItemToCart(),
                c => c.AddItemToCart(),
                c => c.AddItemToCart(),
                c => Task.Delay(1000),
                c => c.GetOrder(),
                c => c.DeleteItemFromCart(),
                c => c.Checkout(),
                c => Task.Delay(1000),
                c => c.GetOrder(),
                c => c.Authorize(),
                c => Task.Delay(10000),
                c => c.GetOrder(),
                c => c.Check()
            );
    }
}