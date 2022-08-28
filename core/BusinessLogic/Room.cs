using core.Networking.Generated;
using Google.Protobuf;
namespace core.BusinessLogic;

public class Room
{
    private readonly List<Account> _accounts = new();
    public string RoomId { get; }
    public bool Empty => _accounts.Count == 0;
    
    public Room(string roomId)
    {
        RoomId = roomId;
    }
    
    public JoinRoomResults Join(Account account)
    {
        lock (_accounts)
        {
            if (!_accounts.Contains(account))
            {
                _accounts.Add(account);
            }

            return JoinRoomResults.JrrDone;
        }
    }

    public void Leave(Account account)
    {
        lock (_accounts)
        {
            if (_accounts.Contains(account))
            {
                _accounts.Remove(account);
            }
        }
    }

    public void Send(IMessage message, params Account [] excludedAccounts)
    {
        lock (_accounts)
        {
            foreach (var account in _accounts)
            {
                if (excludedAccounts.Contains(account) || !account.Client.Active)
                {
                    continue;
                }
                account.Client.Send(message);
            }
        }
    }
}