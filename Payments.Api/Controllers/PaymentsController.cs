using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Payments.Api.Commands.Authorize;
using Payments.Api.Commands.Create;

namespace Payments.Api.Controllers;

[PublicAPI]
public record CreateRequest(string ReferenceId, decimal Amount, int CurrencyCode);

[PublicAPI]
public record CreateResponse(string PaymentId);

[PublicAPI]
public record AuthorizeRequest(string PaymentId);

[PublicAPI]
public record AuthorizeResponse;

[ApiController]
public class PaymentsController : ControllerBase
{
    [HttpPost]
    [Route("payments/create")]
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
    [Route("payments/authorize")]
    public async Task<IActionResult> Authorize(
        [FromServices] IAuthorizeCommand command,
        [FromBody] AuthorizeRequest request,
        CancellationToken ct)
    {
        await command.Execute(request.PaymentId, ct);
        var response = new AuthorizeResponse();
        return Ok(response);
    }
}