using Dapr.Client;

namespace ServerlessCqrs.Host.Modules.Accounts;

public interface IAccountsNotifier
{
    Task Notify(AccountValueIncreasedEvent envelope);
    Task Notify(AccountValueDecreasedEvent envelope);
}

public class AccountsNotifier : IAccountsNotifier
{
    private static readonly string PUBSUB_COMPONENT_NAME = "asb_pub_sub";

    private readonly DaprClient client;

    public AccountsNotifier(DaprClient client)
    {
        this.client = client;
    }

    public Task Notify(AccountValueIncreasedEvent @event)
        => client.PublishEventAsync(PUBSUB_COMPONENT_NAME, typeof(AccountValueIncreasedEvent).Name, @event);

    public Task Notify(AccountValueDecreasedEvent @event)
        => client.PublishEventAsync(PUBSUB_COMPONENT_NAME, typeof(AccountValueDecreasedEvent).Name, @event);
}


