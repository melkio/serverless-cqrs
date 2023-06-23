using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using ServerlessCqrs.Host.Modules.Accounts;

namespace ServerlessCqrs.Host.Modules.Transactions.Transports.Http;

[ApiController]
[Route("api/transactions/subscription")]
public class TransactionSubscriptionsController : ControllerBase
{
    private readonly ITransactionsService service;
    private readonly DaprClient client;

    public TransactionSubscriptionsController(ITransactionsService service, DaprClient client)
    {
        this.service = service;
        this.client = client;
    }

    [HttpPost("activate")]
    [Topic("asb_pub_sub", nameof(TransactionRegisteredEvent))]
    public async Task<IActionResult> Activate(TransactionRegisteredEvent @event)
    {
        var command = new DecreaseAccountValueCommand
        {
            CorrelationId = @event.CorrelationId,
            AccountId = @event.SourceAccountId,
            Amount = @event.Amount
        };
        await client
            .PublishEventAsync("asb_pub_sub", nameof(DecreaseAccountValueCommand), command)
            .ConfigureAwait(false);

        return Ok();
    }

    [HttpPost("source-account-decreased")]
    [Topic("asb_pub_sub", nameof(AccountValueDecreasedEvent))]
    public async Task<IActionResult> SourceAccountDecreased(AccountValueDecreasedEvent @event)
    {
        var (transaction, _) = await service
            .Get(@event.CorrelationId)
            .ConfigureAwait(false);

        var command = new DecreaseAccountValueCommand
        {
            CorrelationId = @event.CorrelationId,
            AccountId = transaction.DestinationAccountId,
            Amount = transaction.Amount
        };
        await client
            .PublishEventAsync("asb_pub_sub", nameof(IncreaseAccountValueCommand), command)
            .ConfigureAwait(false);

        return Ok();
    }

    [HttpPost("destination-account-increased")]
    [Topic("asb_pub_sub", nameof(AccountValueIncreasedEvent))]
    public async Task<IActionResult> DestinationAccountIncreased(AccountValueIncreasedEvent @event)
    {
        var (transaction, _) = await service
            .Get(@event.CorrelationId)
            .ConfigureAwait(false);

        if (transaction != null)
        {
            var command = new CompleteTransactionCommand
            {
                CorrelationId = @event.CorrelationId,
                TransactionId = transaction.Id
            };
            await service
                .Execute(command)
                .ConfigureAwait(false);
        }

        return NoContent();
    }
}