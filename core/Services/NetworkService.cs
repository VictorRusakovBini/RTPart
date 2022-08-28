using System.Net;
using System.Net.Sockets;
using core.Logging;
using core.Networking;

namespace core.Services
{
    public class NetworkService : IService
    {
        private TcpListener _server;
        private readonly List<NetworkClient> _clients = new();

        public bool Active => true;

        public void Initialize()
        {
            _server = new TcpListener(IPAddress.Any, 5577);
            _server.Start();
            Start();
            
            var timer = new Timer((_) =>
            {
                lock (_clients)
                {
                    _clients.RemoveAll((c) => !c.Active);
                }
            });
            timer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
        }

        private void Start()
        {
            var accept = new Task(AcceptLoop);
            accept.Start();
        }

        private async void AcceptLoop()
        {
            while (Active)
            {
                var client = await _server.AcceptTcpClientAsync();

                lock (_clients)
                {
                    var c = new NetworkClient(client);
                    _clients.Add(c);
                    c.Start();
                    Model.Instance.Accounts.AccountCreate(c);
                    Debug.Log("user connected");
                }
            }
        }
    }
}