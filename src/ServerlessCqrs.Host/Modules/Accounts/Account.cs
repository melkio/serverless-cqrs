namespace ServerlessCqrs.Host.Modules.Accounts;

public record Account(Guid Id, int Value);

public class IncreaseAccountValueCommand : CommandBase
{
    public Guid AccountId { get; init; }
    public int Amount { get; init; }
}

public class DecreaseAccountValueCommand : CommandBase
{
    public Guid AccountId { get; init; }
    public int Amount { get; init; }
}

public class AccountValueIncreasedEvent : EventBase
{
    public Guid AccountId { get; init; }
    public int Amount { get; init; }
}

public class AccountValueDecreasedEvent : EventBase
{
    public Guid AccountId { get; init; }
    public int Amount { get; init; }
}




