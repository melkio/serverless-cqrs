using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace ServerlessCqrs.Host.Modules.Transactions.Transports.Http;

[ApiController]
[Route("api/transactions/summary/subscriptions")]
public class SummarySubscriptionsController : ControllerBase
{
    private static readonly string STATE_STORE = "transactions_summary_state_store";

    private readonly ITransactionsService service;
    private readonly DaprClient client;

    public SummarySubscriptionsController(ITransactionsService service, DaprClient client)
    {
        this.service = service;
        this.client = client;
    }

    [HttpPost("update-summary")]
    [Topic("asb_pub_sub", nameof(TransactionCompletedEvent))]
    public async Task<IActionResult> UpdateSummary(TransactionCompletedEvent @event)
    {
        var (transaction, _) = await service
            .Get(@event.TransactionId)
            .ConfigureAwait(false);

        var task1 = client.GetStateAsync<TransactionSummary>(STATE_STORE, $"{transaction.SourceAccountId}");
        var task2 = client.GetStateAsync<TransactionSummary>(STATE_STORE, $"{transaction.DestinationAccountId}");
        await Task.WhenAll(task1, task2).ConfigureAwait(false);

        var summary1 = task1.Result ?? new TransactionSummary(transaction.SourceAccountId, 0);
        var summary2 = task2.Result ?? new TransactionSummary(transaction.DestinationAccountId, 0);

        var task3 = client.SaveStateAsync(
            STATE_STORE, 
            $"{transaction.SourceAccountId}", 
            summary1 with {CompletedTransactions = summary1.CompletedTransactions + 1});
        var task4 = client.SaveStateAsync(
            STATE_STORE, 
            $"{transaction.DestinationAccountId}", 
            summary2 with {CompletedTransactions = summary2.CompletedTransactions + 1});
        await Task.WhenAll(task3, task4).ConfigureAwait(false);

        return Ok();
    }
}