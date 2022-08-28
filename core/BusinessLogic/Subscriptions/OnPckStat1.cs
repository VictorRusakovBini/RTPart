using core.Networking.Generated;
using core.Networking.Subscriptions;

namespace core.BusinessLogic.Subscriptions;

public class OnPckStat1 : SubscriptionHandle<PckSendStat1>
{
    public OnPckStat1(Account account) : base(account)
    {
    }

    protected override void Process(PckSendStat1 message)
    {   
        Account.Room?.Send(message, Account);
    }
}