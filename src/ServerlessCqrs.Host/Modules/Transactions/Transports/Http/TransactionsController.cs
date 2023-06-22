using Microsoft.AspNetCore.Mvc;

namespace ServerlessCqrs.Host.Modules.Transactions.Transports.Http;

[ApiController]
[Route("api/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionsService service;

    public TransactionsController(ITransactionsService service)
    {
        this.service = service;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var (transaction, _) = await service
            .Get(id)
            .ConfigureAwait(false);

        return transaction switch
        {
            null => NotFound(),
            _ => Ok(transaction)
        };
    }

    [HttpPost]
    public async Task<IActionResult> Post(PostTransactionModel model)
    {
        var transactionId = Guid.NewGuid();
        var command = new RegisterTransactionCommand
        {
            CorrelationId = transactionId,
            TransactionId = transactionId,
            SourceAccountId = model.SourceAccountId,
            DestinationAccountId = model.DestinationAccountId,
            Amount = model.Amount
        };
        await service    
            .Execute(command)
            .ConfigureAwait(false);

        return Created($"api/transactions/{command.TransactionId}", new { Id = command.TransactionId });
    }
}