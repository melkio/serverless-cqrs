using Microsoft.AspNetCore.Mvc;

namespace ServerlessCqrs.Host.Modules.Accounts.Transports.Http;

[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly IAccountsService service;

    public AccountsController(IAccountsService service)
    {
        this.service = service;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var (account, _) = await service
            .Get(id)
            .ConfigureAwait(false);

        return account switch
        {
            null => NotFound(),
            _ => Ok(account)
        };
    }

    [HttpPost("increase")]
    public async Task<IActionResult> Increase(IncreaseAccountValueCommand command)
    {
        await service
            .Execute(command)
            .ConfigureAwait(false);

        return NoContent();
    }

    [HttpPost("decrease")]
    public async Task<IActionResult> Decrease(DecreaseAccountValueCommand command)
    {
        await service
            .Execute(command)
            .ConfigureAwait(false);

        return NoContent();
    }
}