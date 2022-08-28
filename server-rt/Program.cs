using core;
using core.Logging;

namespace server_rt
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Model.Instance.Initialize();
            
            Debug.Log("server started");
            while (Model.Instance.Networking.Active)
            {
                await Task.Delay(1000);
            }
        }
    }
}