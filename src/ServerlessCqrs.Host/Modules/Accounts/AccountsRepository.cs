using Dapr.Client;

namespace ServerlessCqrs.Host.Modules.Accounts;

public interface IAccountsRepository
{
    Task<(Account, string)> Get(Guid id);
    Task Upsert(Account account, string etag = null);
}

public class AccountsRepository : IAccountsRepository
{
    private static readonly string STATE_STORE = "accounts_state_store";

    private readonly DaprClient client;

    public AccountsRepository(DaprClient client)
    {
        this.client = client;
    }

    public Task<(Account, string)> Get(Guid id)
        => client.GetStateAndETagAsync<Account>(STATE_STORE, $"{id}");

    public async Task Upsert(Account account, string etag)
    {
        var task = etag switch
        {
            null => UpsertWithoutVersion(account),
            _ => UpsertWithVersion(account, etag)
        };

        var result = await task.ConfigureAwait(false);
        if (!result)
            throw new InvalidOperationException("Account was modified by another process");
    }

    private async Task<bool> UpsertWithoutVersion(Account account)
    {
        await client.SaveStateAsync(STATE_STORE, $"{account.Id}", account);
        return true;
    }

    private Task<bool> UpsertWithVersion(Account account, string etag)
        => client.TrySaveStateAsync(STATE_STORE, $"{account.Id}", account, etag);
}


