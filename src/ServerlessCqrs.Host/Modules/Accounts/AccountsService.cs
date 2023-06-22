namespace ServerlessCqrs.Host.Modules.Accounts;

public interface IAccountsService
{
    Task<(Account, string)> Get(Guid id);
    Task Execute(IncreaseAccountValueCommand command);
    Task Execute(DecreaseAccountValueCommand command);
}

public class AccountsService : IAccountsService
{
    private readonly IAccountsRepository repository;
    private readonly IAccountsNotifier notifier;

    public AccountsService(IAccountsRepository repository, IAccountsNotifier notifier)
    {
        this.repository = repository;
        this.notifier = notifier;
    }

    public Task<(Account, string)> Get(Guid id)
        => repository.Get(id);

    public async Task Execute(IncreaseAccountValueCommand command)
    {
        var (account, etag) = await repository
            .Get(command.AccountId)
            .ConfigureAwait(false);

        var newAccount = (account ??= new Account(command.AccountId, 0)) 
            with { Value = account.Value + command.Amount };

        await repository
            .Upsert(newAccount, etag)
            .ConfigureAwait(false);

        var @event = new AccountValueIncreasedEvent
        {
            CorrelationId = command.CorrelationId,
            AccountId = command.AccountId,
            Amount = command.Amount
        };
        await notifier
            .Notify(@event)
            .ConfigureAwait(false);
    }

    public async Task Execute(DecreaseAccountValueCommand command)
    {
        var (account, etag) = await repository
            .Get(command.AccountId)
            .ConfigureAwait(false);

        if (account is null)
            throw new InvalidOperationException("Account does not exist");

        if (account.Value < command.Amount)
            throw new InvalidOperationException("Account value is too low");

        var newAccount = account with { Value = account.Value - command.Amount };

        await repository
            .Upsert(newAccount, etag)
            .ConfigureAwait(false);

        var @event = new AccountValueDecreasedEvent
        {
            CorrelationId = command.CorrelationId,
            AccountId = command.AccountId,
            Amount = command.Amount
        };
        await notifier
            .Notify(@event)
            .ConfigureAwait(false);
    }
}


