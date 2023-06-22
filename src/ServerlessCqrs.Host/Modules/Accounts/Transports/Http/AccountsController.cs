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

    [HttpPost]
    public async Task<IActionResult> Post(PostAccountModel model)
    {
        var command = new IncreaseAccountValueCommand
        {
            AccountId = Guid.NewGuid(),
            Amount = model.Amount
        };
        await service
            .Execute(command)
            .ConfigureAwait(false);

        return Created($"api/accounts/{command.AccountId}", new { Id = command.AccountId });
    }
}