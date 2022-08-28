using core.Networking.Generated;
using core.Networking.Subscriptions;

namespace core.BusinessLogic.Subscriptions;

public class OnPckStat3 : SubscriptionHandle<PckSendStat3>
{
    public OnPckStat3(Account account) : base(account)
    {
    }

    protected override void Process(PckSendStat3 message)
    {
        Account.Room?.Send(message);
    }
}