using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace ServerlessCqrs.Host.Modules.Transactions.Transports.Http;

[ApiController]
[Route("api/transactions/summary")]
public class TransactionsSummaryController : ControllerBase
{
    private static readonly string STATE_STORE = "transactions_summary_state_store";

    private readonly DaprClient client;

    public TransactionsSummaryController(DaprClient client)
    {
        this.client = client;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var summary = await client
            .GetStateAsync<TransactionSummary>(STATE_STORE, $"{id}")
            .ConfigureAwait(false);

        return Ok(summary);
    }
}