using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Payments.Api.Commands.Authorize;
using Payments.Api.Commands.Create;
using Payments.Api.Queries.Get;
using Payment = Payments.Domain.Payment;

namespace Payments.Api.Controllers;

[PublicAPI]
public record CreateRequest(string ReferenceId, decimal Amount, int CurrencyCode);

[PublicAPI]
public record CreateResponse(string PaymentId);

[PublicAPI]
public record AuthorizeRequest(string PaymentId);

[PublicAPI]
public record AuthorizeResponse;

[PublicAPI]
public record GetRequest(string PaymentId);

[PublicAPI]
public record GetResponse(GetResponse.PaymentDto Payment)
{
    [PublicAPI]
    public record PaymentDto(string Id, string PaymentStatus);
}

[ApiController]
[Route("payments")]
public class PaymentsController : ControllerBase
{
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create(
        [FromServices] ICreateCommand command,
        [FromBody] CreateRequest request,
        CancellationToken ct)
    {
        var result = await command.Execute(request.ReferenceId, request.Amount, request.CurrencyCode, ct);
        var response = new CreateResponse(result.PaymentId);
        return Ok(response);
    }

    [HttpPost]
    [Route("authorize")]
    public async Task<IActionResult> Authorize(
        [FromServices] IAuthorizeCommand command,
        [FromBody] AuthorizeRequest request,
        CancellationToken ct)
    {
        await command.Execute(request.PaymentId, ct);
        var response = new AuthorizeResponse();
        return Ok(response);
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Get(
        [FromServices] IGetQuery query,
        [FromBody] GetRequest request,
        CancellationToken ct)
    {
        var result = await query.Execute(request.PaymentId, ct);
        var response = new GetResponse(
            Payment: new GetResponse.PaymentDto(
                Id: result.Payment.Id,
                PaymentStatus: result.Payment.PaymentStatus
            )
        );
        return Ok(response);
    }
}