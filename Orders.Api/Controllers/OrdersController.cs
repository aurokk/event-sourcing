using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Orders.Api.Commands.AddToCart;
using Orders.Api.Commands.Checkout;
using Orders.Api.Commands.Create;
using Orders.Api.Commands.DeleteFromCart;
using Orders.Api.Queries.Get;

namespace Orders.Api.Controllers;

#region Dto

[PublicAPI]
public record CreateRequest;

[PublicAPI]
public record CreateResponse(string OrderId);

[PublicAPI]
public record AddToCartRequest(string OrderId, string ProductId);

[PublicAPI]
public record AddToCartResponse;

[PublicAPI]
public record DeleteFromCartRequest(string OrderId, string CartItemId);

public record DeleteFromCartResponse;

[PublicAPI]
public record CheckoutRequest(string OrderId);

[PublicAPI]
public record CheckoutResponse;

[PublicAPI]
public record GetRequest(string OrderId);

[PublicAPI]
public record GetResponse(GetResponse.OrderDto Order)
{
    [PublicAPI]
    public record OrderDto(string Id, OrderDto.CartItemDto[] Cart, string OrderStatus, string? PaymentId)
    {
        [PublicAPI]
        public record CartItemDto(string Id, string ProductId);
    }
}

#endregion

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    [HttpPost]
    [Route("create")]
    [ProducesResponseType(typeof(CreateResponse), 200)]
    public async Task<IActionResult> Create(
        [FromServices] ICreateCommand command,
        [FromBody] CreateRequest request,
        CancellationToken ct)
    {
        var commandResponse = await command.Execute(ct);
        var response = new CreateResponse(OrderId: commandResponse.OrderId);
        return Ok(response);
    }

    [HttpPost]
    [Route("addtocart")]
    [ProducesResponseType(typeof(AddToCartResponse), 200)]
    public async Task<IActionResult> AddToCart(
        [FromServices] IAddToCartCommand command,
        [FromBody] AddToCartRequest request,
        CancellationToken ct)
    {
        var commandRequest = new AddToCartCommandRequest(request.OrderId, request.ProductId);
        var commandResponse = await command.Execute(commandRequest, ct);
        var response = new AddToCartResponse();
        return Ok(response);
    }

    [HttpPost]
    [Route("deletefromcart")]
    [ProducesResponseType(typeof(DeleteFromCartResponse), 200)]
    public async Task<IActionResult> DeleteFromCart(
        [FromServices] IDeleteFromCartCommand command,
        [FromBody] DeleteFromCartRequest request,
        CancellationToken ct)
    {
        var commandRequest = new DeleteFromCartCommandRequest(request.OrderId, request.CartItemId);
        var commandResponse = await command.Execute(commandRequest, ct);
        var response = new DeleteFromCartResponse();
        return Ok(response);
    }

    [HttpPost]
    [Route("checkout")]
    [ProducesResponseType(typeof(CheckoutResponse), 200)]
    public async Task<IActionResult> Checkout(
        [FromServices] ICheckoutCommand command,
        [FromBody] CheckoutRequest request,
        CancellationToken ct)
    {
        var commandRequest = new CheckoutCommandRequest(request.OrderId);
        var commandResponse = await command.Execute(commandRequest, ct);
        var response = new CheckoutResponse();
        return Ok(response);
    }

    [HttpGet]
    [Route("")]
    [ProducesResponseType(typeof(GetResponse), 200)]
    public async Task<IActionResult> Get(
        [FromServices] IGetQuery query,
        [FromQuery] GetRequest request,
        CancellationToken ct)
    {
        var queryRequest = new Orders.Api.Queries.Get.GetRequest(request.OrderId);
        var queryResponse = await query.Execute(queryRequest, ct);
        var response = new GetResponse(
            Order: new GetResponse.OrderDto(
                Id: queryResponse.Order.Id,
                Cart: queryResponse.Order.Cart
                    .Select(x => new GetResponse.OrderDto.CartItemDto(x.Id, x.ProductId))
                    .ToArray(),
                PaymentId: queryResponse.Order.PaymentId,
                OrderStatus: queryResponse.Order.OrderStatus
            )
        );
        return Ok(response);
    }
}