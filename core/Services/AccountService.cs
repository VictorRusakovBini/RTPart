using core.BusinessLogic;
using core.Networking;
using core.Networking.Subscriptions;

namespace core.Services;

public class AccountService : IService
{
    private readonly List<Account> _accounts = new();
    public void AccountCreate(NetworkClient client)
    {
        lock (_accounts)
        {
            var acc = new Account(client);
            NetworkSubscriptionManager.Subscribe(acc);
            
            _accounts.Add(acc);
        }
    }

    public void Initialize()
    {
        var timer = new Timer((_) =>
        {
            lock (_accounts)
            {
                _accounts.RemoveAll(a => !a.Client.Active);
            }

        });
        timer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
    }
}