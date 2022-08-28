using core.Networking.Subscriptions;
using Google.Protobuf;

namespace core.Networking
{
    public abstract class NetworkClientBase
    {
        protected readonly Dictionary<Type, List<Action<IMessage>>> Listeners = new();
        private readonly List<ISubscriptionHandleProxy> _registeredProxy = new();
        public event Action OnConnectionBroken = () => { };
        public abstract void Send(IMessage message);
        public abstract void Send(byte [] message, Type messageType = null);

        public virtual void Disconnect(bool fireCallback)
        {
            UnsubscribeAll();
        }

        public bool Active { get; protected set; }

        public void EmulatePacket(IMessage message)
        {
            lock (Listeners)
            {
                var T =  message.GetType();
                if (Listeners.ContainsKey(T))
                {
                    ProcessActionsList(message, Listeners[T]);
                }
            }
        }

        public void RegisterListener<T>(Action<IMessage> callback)  where T : IMessage
        {
            lock (Listeners)
            {
                var t = typeof(T);
                if (!Listeners.ContainsKey(t))
                {
                    Listeners.Add(t, new List<Action<IMessage>>());
                }
                Listeners[t].Add(callback);
            }
        }
        
        public void RemoveListener<T>(Action<IMessage> callback) where T : IMessage
        {
            lock (Listeners)
            {
                var t = typeof(T);
                if (Listeners.ContainsKey(t))
                {
                    Listeners[t].Remove(callback);
                }
            }
        }

        protected void InvokeOnConnectionBroken()
        {
            OnConnectionBroken?.Invoke();
        }
        
        protected void ProcessActionsList(IMessage message, List<Action<IMessage>> actionsList)
        {
            for (int i = actionsList.Count - 1; i >= 0; i--)
            {
                actionsList[i].Invoke(message);
            }
        }
        
        public void RegisterSubscriptionHandler(ISubscriptionHandleProxy handler)
        {
            _registeredProxy.Add(handler);
            handler.Subscribe();
        }
        
        public void UnsubscribeAll()
        {
            foreach (var subscriptionHandleProxy in _registeredProxy)
            {
                subscriptionHandleProxy?.Unsubscribe();
            }
            
            _registeredProxy.Clear();
        }
    }
}