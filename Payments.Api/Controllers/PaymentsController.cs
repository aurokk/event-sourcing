using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Payments.Api.Commands.Authorize;
using Payments.Api.Commands.Create;

namespace Payments.Api.Controllers;

[PublicAPI]
public record CreateRequest;

[PublicAPI]
public record CreateResponse(string PaymentId);

[PublicAPI]
public record AuthorizeRequest(string PaymentId);

[PublicAPI]
public record AuthorizeResponse;

[ApiController]
public class PaymentsController : ControllerBase
{
    public async Task<IActionResult> Create(
        [FromServices] ICreateCommand command,
        CancellationToken ct)
    {
        var result = await command.Execute(ct);
        var response = new CreateResponse(result.PaymentId);
        return Ok(response);
    }

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