using Microsoft.AspNetCore.Mvc;
using Orders.Api.Commands.AddToCart;
using Orders.Api.Commands.Checkout;
using Orders.Api.Commands.Create;
using Orders.Api.Commands.DeleteFromCart;
using Orders.Api.Queries.Get;

namespace Orders.Api.Controllers;

#region Dto

public record CreateRequest;

public record CreateResponse(string OrderId);

public record AddToCartRequest(string OrderId, string ProductId);

public record AddToCartResponse;

public record DeleteFromCartRequest(string OrderId, string CartItemId);

public record DeleteFromCartResponse;

public record CheckoutRequest(string OrderId);

public record CheckoutResponse;

public record GetRequest(string OrderId);

public record GetResponse(GetResponse.OrderDto Order)
{
    public record OrderDto(string Id);
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
                Id: queryResponse.Order.Id
            )
        );
        return Ok(response);
    }
}