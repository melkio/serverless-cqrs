namespace ServerlessCqrs.Host.Modules.Transactions;

public static class TransactionsConfiguration
{
    public static IServiceCollection AddTransactionComponents(this IServiceCollection services)
    {
        services.AddSingleton<ITransactionsRepository, TransactionsRepository>();
        services.AddSingleton<ITransactionsNotifier, TransactionsNotifier>();
        services.AddSingleton<ITransactionsService, TransactionsService>();

        return services;
    }
}