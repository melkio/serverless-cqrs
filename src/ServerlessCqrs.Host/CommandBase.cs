namespace ServerlessCqrs.Host;

public abstract class CommandBase
{
    public Guid CorrelationId { get; init; }
}
