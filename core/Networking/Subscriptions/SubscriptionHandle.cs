using core.BusinessLogic;
using core.Logging;
using Google.Protobuf;
using Newtonsoft.Json;

namespace core.Networking.Subscriptions
{
    public abstract class SubscriptionHandle<T> : ISubscriptionHandleProxy where T : IMessage
    {
        protected Account Account { get; }

        protected void Send(IMessage message)
        {
            Account.Client.Send(message);
        }

        protected SubscriptionHandle(Account account)
        {
            Account = account;
        }

        public void Subscribe()
        {
            Account.Client.RegisterListener<T>(Handle);
        }

        public void Unsubscribe()
        {
            Account.Client.RemoveListener<T>(Handle);
        }

        private void Handle(IMessage message)
        {
            Process((T)message);
        }

        protected abstract void Process(T message);
    }
}