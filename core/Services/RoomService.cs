using core.BusinessLogic;

namespace core.Services;

public class RoomService: IService
{
    private readonly Dictionary<string, Room> _rooms = new();

    public Room GetRoom(string roomId)
    {
        lock (_rooms)
        {
            if (!_rooms.ContainsKey(roomId))
            {
                _rooms.Add(roomId, new Room(roomId));
            }

            return _rooms[roomId];
        }
    }

    public void Initialize()
    {
        var timer = new Timer((_) =>
        {
            lock (_rooms)
            {
                var emptyRooms = _rooms.Values.Where(r => r.Empty).ToList();
                foreach (var emptyRoom in emptyRooms)
                {
                    _rooms.Remove(emptyRoom.RoomId);
                }
            }
        });
        timer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));

    }
}