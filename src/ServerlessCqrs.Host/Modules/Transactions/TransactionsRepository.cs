using Dapr.Client;

namespace ServerlessCqrs.Host.Modules.Transactions;

public interface ITransactionsRepository
{
    Task<(Transaction, string)> Get(Guid id);
    Task Upsert(Transaction transaction, string etag = null);
}

public class TransactionsRepository : ITransactionsRepository
{
    private static readonly string STATE_STORE = "transactions_state_store";

    private readonly DaprClient client;

    public TransactionsRepository(DaprClient client)
    {
        this.client = client;
    }

    public Task<(Transaction, string)> Get(Guid id)
        => client.GetStateAndETagAsync<Transaction>(STATE_STORE, $"{id}");

    public async Task Upsert(Transaction transaction, string etag)
    {
        var task = etag switch
        {
            null => UpsertWithoutVersion(transaction),
            _ => UpsertWithVersion(transaction, etag)
        };

        var result = await task.ConfigureAwait(false);
        if (!result)
            throw new InvalidOperationException("Transaction was modified by another process");
    }

    private async Task<bool> UpsertWithoutVersion(Transaction transaction)
    {
        await client.SaveStateAsync(STATE_STORE, $"{transaction.Id}", transaction);
        return true;
    }

    private Task<bool> UpsertWithVersion(Transaction transaction, string etag)
        => client.TrySaveStateAsync(STATE_STORE, $"{transaction.Id}", transaction, etag);
}


