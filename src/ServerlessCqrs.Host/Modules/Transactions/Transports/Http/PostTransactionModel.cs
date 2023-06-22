using System.ComponentModel.DataAnnotations;

namespace ServerlessCqrs.Host.Modules.Transactions.Transports.Http;

public class PostTransactionModel
{
    public Guid SourceAccountId { get; init; }
    public Guid DestinationAccountId { get; init; }
    [Range(1, 1000)]
    public int Amount { get; init; }
}