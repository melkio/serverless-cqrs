namespace ServerlessCqrs.Host.Modules.Transactions;

public record Transaction(Guid Id, Guid SourceAccountId, Guid DestinationAccountId, int Amount);

public class RegisterTransactionCommand : CommandBase
{
    public Guid TransactionId { get; init; }
    public Guid SourceAccountId { get; init; }
    public Guid DestinationAccountId { get; init; }
    public int Amount { get; init; }
}

public class TransactionRegisteredEvent : EventBase
{
    public Guid TransactionId { get; init; }
    public Guid SourceAccountId { get; init; }
    public Guid DestinationAccountId { get; init; }
    public int Amount { get; init; }
}