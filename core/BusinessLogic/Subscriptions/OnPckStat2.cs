using core.Networking.Generated;
using core.Networking.Subscriptions;

namespace core.BusinessLogic.Subscriptions;

public class OnPckStat2 : SubscriptionHandle<PckSendStat2>
{
    public OnPckStat2(Account account) : base(account)
    {
    }

    protected override void Process(PckSendStat2 message)
    {
        Account.Room?.Send(message, Account);
    }
}