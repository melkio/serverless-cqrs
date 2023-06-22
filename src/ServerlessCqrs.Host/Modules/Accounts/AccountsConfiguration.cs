namespace ServerlessCqrs.Host.Modules.Accounts;

public static class AccountsConfiguration
{
    public static IServiceCollection AddAccountComponents(this IServiceCollection services)
    {
        services.AddSingleton<IAccountsRepository, AccountsRepository>();
        services.AddSingleton<IAccountsNotifier, AccountsNotifier>();
        services.AddSingleton<IAccountsService, AccountsService>();

        return services;
    }
}