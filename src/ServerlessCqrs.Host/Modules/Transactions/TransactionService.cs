namespace ServerlessCqrs.Host.Modules.Transactions;

public interface ITransactionsService
{
    Task<(Transaction, string)> Get(Guid id);
    Task Execute(RegisterTransactionCommand command);
}

public class TransactionsService : ITransactionsService
{
    private readonly ITransactionsRepository repository;
    private readonly ITransactionsNotifier notifier;

    public TransactionsService(ITransactionsRepository repository, ITransactionsNotifier notifier)
    {
        this.repository = repository;
        this.notifier = notifier;
    }

    public Task<(Transaction, string)> Get(Guid id)
        => repository.Get(id);

    public async Task Execute(RegisterTransactionCommand command)
    {
        var (transaction, _) = await repository
            .Get(command.TransactionId)
            .ConfigureAwait(false);

        if (transaction is not null)
            throw new InvalidOperationException("Transaction already exists");

        transaction = new Transaction(command.TransactionId, command.SourceAccountId, command.DestinationAccountId, command.Amount);

        await repository
            .Upsert(transaction)
            .ConfigureAwait(false);

        var @event = new TransactionRegisteredEvent
        {
            CorrelationId = command.CorrelationId,
            TransactionId = command.TransactionId,
            SourceAccountId = command.SourceAccountId,
            DestinationAccountId = command.DestinationAccountId,
            Amount = command.Amount
        };
        await notifier
            .Notify(@event)
            .ConfigureAwait(false);
    }
}



