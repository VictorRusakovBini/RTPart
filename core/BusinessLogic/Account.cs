using core.Networking;
using core.Networking.Generated;

namespace core.BusinessLogic;

public class Account
{
    public NetworkClient Client { get; }
    public Room Room { get; private set; }
    public int UserId { get; private set; }

    public Account(NetworkClient client)
    {
        Client = client;
        Client.SetAccount(this);
        client.OnConnectionBroken += OnConnectionBroken;
    }

    private void OnConnectionBroken()
    {
        Room?.Leave(this);
    }

    public JoinRoomResults JoinRoom(string roomId, int userId)
    {
        UserId = userId;
        
        var room = Model.Instance.Rooms.GetRoom(roomId);
        var result = room.Join(this);
        
        if (result == JoinRoomResults.JrrDone)
        {
            Room = room;
        }
        
        return result;
    }
}