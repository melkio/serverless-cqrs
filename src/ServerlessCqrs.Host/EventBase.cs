namespace ServerlessCqrs.Host;

public abstract class EventBase
{
    public Guid CorrelationId { get; init; }
}