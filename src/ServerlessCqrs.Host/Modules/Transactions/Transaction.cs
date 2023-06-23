namespace ServerlessCqrs.Host.Modules.Transactions;

public enum TransactionStatus
{
    Pending,
    Completed
}

public record Transaction(Guid Id, Guid SourceAccountId, Guid DestinationAccountId, int Amount, TransactionStatus Status);

public record TransactionSummary(Guid AccountId, int CompletedTransactions);

public class RegisterTransactionCommand : CommandBase
{
    public Guid TransactionId { get; init; }
    public Guid SourceAccountId { get; init; }
    public Guid DestinationAccountId { get; init; }
    public int Amount { get; init; }
}

public class CompleteTransactionCommand : CommandBase
{
    public Guid TransactionId { get; init; }
}

public class TransactionRegisteredEvent : EventBase
{
    public Guid TransactionId { get; init; }
    public Guid SourceAccountId { get; init; }
    public Guid DestinationAccountId { get; init; }
    public int Amount { get; init; }
}

public class TransactionCompletedEvent : EventBase
{
    public Guid TransactionId { get; init; }
}