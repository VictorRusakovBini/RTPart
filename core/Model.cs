using core.Logging;
using core.Services;
using Server.Core.Utility.Logging;

namespace core;

public class Model
{
    public readonly RoomService Rooms = new();
    public readonly NetworkService Networking = new();
    public readonly AccountService Accounts = new();

    public static Model Instance { get; } = new ();

    private Model() { }

    public void Initialize()
    {
        Debug.Initialize<LokiLogger>();
        Rooms.Initialize();
        Accounts.Initialize();
        Networking.Initialize();
    }
}