using Google.Protobuf;

namespace core.Networking
{
    public interface INetArray
    {
        int Length { get; }
        void Write(byte[] data, int? offset = null, int? count = null);
        short ReadShort();
        IMessage GetData(MessageParser parser, short len);
    }
}