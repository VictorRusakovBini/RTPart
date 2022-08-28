using core.Networking.Generated;
using core.Networking.Subscriptions;

namespace core.BusinessLogic.Subscriptions;

public class OnPckStat4 : SubscriptionHandle<PckSendStat4>
{
    public OnPckStat4(Account account) : base(account)
    {
    }

    protected override void Process(PckSendStat4 message)
    {
        Account.Room?.Send(message);
    }
}