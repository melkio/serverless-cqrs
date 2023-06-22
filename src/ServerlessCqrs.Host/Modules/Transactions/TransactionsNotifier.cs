using Dapr.Client;

namespace ServerlessCqrs.Host.Modules.Transactions;

public interface ITransactionsNotifier
{
    Task Notify(TransactionRegisteredEvent envelope);
}

public class TransactionsNotifier : ITransactionsNotifier
{
    private static readonly string PUBSUB_COMPONENT_NAME = "asb_pub_sub";

    private readonly DaprClient client;

    public TransactionsNotifier(DaprClient client)
    {
        this.client = client;
    }

    public Task Notify(TransactionRegisteredEvent @event)
        => client.PublishEventAsync(PUBSUB_COMPONENT_NAME, typeof(TransactionRegisteredEvent).Name, @event);
}


