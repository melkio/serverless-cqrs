using Dapr;
using Microsoft.AspNetCore.Mvc;

namespace ServerlessCqrs.Host.Modules.Accounts.Transports.Http;

[ApiController]
[Route("api/accounts/subscriptions")]
public class AccountSubscriptionsController : ControllerBase
{
    private readonly IAccountsService service;

    public AccountSubscriptionsController(IAccountsService service)
    {
        this.service = service;
    }

    [HttpPost("increase")]
    [Topic("asb_pub_sub", nameof(IncreaseAccountValueCommand))]
    public async Task<IActionResult> Increase(IncreaseAccountValueCommand command)
    {
        await service
            .Execute(command)
            .ConfigureAwait(false);

        return NoContent();
    }

    [HttpPost("decrease")]
    [Topic("asb_pub_sub", nameof(DecreaseAccountValueCommand))]
    public async Task<IActionResult> Decrease(DecreaseAccountValueCommand command)
    {
        await service
            .Execute(command)
            .ConfigureAwait(false);

        return NoContent();
    }
}