using Microsoft.AspNetCore.Mvc;
using Orders.Api.Commands;
using Orders.Api.Commands.AddToCart;
using Orders.Api.Commands.Checkout;
using Orders.Api.Commands.DeleteFromCart;
using Orders.Api.Queries.Get;

namespace Orders.Api.Api;

#region Dto

public record CreateRequest;

public record CreateResponse(Guid Id);

public record AddToCartRequest();

public record AddToCartResponse();

public record DeleteFromCartRequest();

public record DeleteFromCartResponse();

public record CheckoutRequest();

public record CheckoutResponse();

public record GetRequest();

public record GetResponse();

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
        await command.Execute();
        var response = new CreateResponse(Id: Guid.NewGuid());
        return Ok();
    }

    [HttpPost]
    [Route("addToCart")]
    [ProducesResponseType(typeof(AddToCartResponse), 200)]
    public async Task<IActionResult> AddToCart(
        [FromServices] IAddToCardCommand command,
        [FromBody] AddToCartRequest request,
        CancellationToken ct)
    {
        await command.Execute();
        var response = new AddToCartResponse();
        return Ok();
    }

    [HttpPost]
    [Route("deleteFromCart")]
    [ProducesResponseType(typeof(DeleteFromCartResponse), 200)]
    public async Task<IActionResult> DeleteFromCart(
        [FromServices] IDeleteFromCardCommand command,
        [FromBody] DeleteFromCartRequest request,
        CancellationToken ct)
    {
        await command.Execute();
        var response = new DeleteFromCartResponse();
        return Ok();
    }

    [HttpPost]
    [Route("checkout")]
    [ProducesResponseType(typeof(CheckoutResponse), 200)]
    public async Task<IActionResult> Checkout(
        [FromServices] ICheckoutCommand command,
        [FromBody] CheckoutRequest request,
        CancellationToken ct)
    {
        await command.Execute();
        var response = new CheckoutResponse();
        return Ok();
    }

    [HttpGet]
    [Route("")]
    [ProducesResponseType(typeof(GetResponse), 200)]
    public async Task<IActionResult> Get(
        [FromServices] IGetQuery query,
        [FromQuery] GetRequest request,
        CancellationToken ct)
    {
        await query.Execute();
        var response = new GetResponse();
        return Ok();
    }
}