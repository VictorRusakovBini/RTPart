using core.Networking.Generated;
using core.Networking.Subscriptions;

namespace core.BusinessLogic.Subscriptions;

public class OnPckConnect : SubscriptionHandle<PckConnect>
{
    public OnPckConnect(Account account) : base(account)
    {
    }

    protected override void Process(PckConnect message)
    {
        var result = Account.JoinRoom(message.RoomId, message.UserId);
        
        Send(new PckConnectResult
        {
            Result = result
        });
    }
}