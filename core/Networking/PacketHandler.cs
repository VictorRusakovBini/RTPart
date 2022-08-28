using System.Net.Sockets;
using core.Logging;
using core.Networking.Generated;
using Google.Protobuf;
using Newtonsoft.Json;

namespace core.Networking
{
    public class PacketsHandler
    {
        private const int HEADER_LENGTH = 4;
        private readonly NetworkStream _stream;
        private readonly INetArray _handle;
        private readonly Task _read;
        private readonly Task _write;
        private NetworkClient _client;
        private readonly byte [] _readBuffer = new byte[2048];
        private short? _awaitedLength = null;
        private short? _awaitedPack = null;

        private readonly Action<IMessage> _onMessage;
        private readonly Action<bool> _onDisconnect;
        private bool _disconnectHandled;
        
        private readonly Queue<object> _sendQueue = new();
        
        public PacketsHandler(NetworkClient client,NetworkStream stream, Action<bool> onDisconnect, Action<IMessage> onMessage)
        {
            _onDisconnect = onDisconnect;
            _onMessage = onMessage;
            _stream = stream;
            _client = client;
            _handle = new FastArray();

            _read = new Task(() =>
            {
                while (client.Active)
                {
                    var bytesRead = 0;
                    try
                    {
                        bytesRead =  _stream.Read(_readBuffer);
                    }
                    catch (Exception)
                    {
                        HandleDisconnect();
                        continue;
                    }
                    
                    if (bytesRead > 0)
                    {
                        _handle.Write(_readBuffer, 0, bytesRead);
                        ProcessPacket();
                    }
                    else
                    {
                        HandleDisconnect();
                    }
                }
            });
            
            _write = new Task( async () =>
            {
                while (_client.Active)
                {
                    if (_sendQueue.Count == 0)
                    {
                        await Task.Delay(50);
                        continue;
                    }

                    object message = null;
                    
                    lock (_sendQueue)
                    {
                        if (_sendQueue.Count > 0)
                        {
                            message = _sendQueue.Dequeue();
                        }
                    }

                    if (message != null)
                    {
                        if (message is IMessage mes)
                        {
                            try
                            {
                                var pack = mes.ToByteArray();
                                WriteToStream(stream, PacketsWrapper.GetPacketIdBytes(mes));
                                WriteToStream(stream, PacketsWrapper.GetPacketLengthBytes((short)pack.Length));
                                WriteToStream(stream, pack);
                            }
                            catch
                            {
                                HandleDisconnect();
                            }
                        }
                        else
                        {
                            try
                            {
                                _stream.Write((byte[])message);
                            }
                            catch 
                            {
                                HandleDisconnect();
                            }
                        }
                    }
                }
               
            });
            
            _read.Start();
            _write.Start();
        }
        
        private void ProcessPacket()
        {
            try
            {
                if (_awaitedLength == null || _awaitedPack == null)
                {
                    if (_handle.Length < HEADER_LENGTH)
                    {
                        return;
                    }
                    
                    _awaitedPack = _handle.ReadShort();
                    _awaitedLength = _handle.ReadShort();
                }

                var hl = _handle.Length;
                if (_handle.Length >= _awaitedLength)
                {
                    var packetId = (PacketsIds)_awaitedPack.Value;
                    try
                    {
                        var message = _handle.GetData(PacketsWrapper.GetPacketType(packetId), _awaitedLength.Value);
                        Handle(message);
                    }
                    catch (Exception)
                    {
                        Debug.Error($"packetId:{packetId}, _awaitedLength: {_awaitedLength}, _handle.Length {hl} ");
                    }
                
                    _awaitedPack = null;
                    _awaitedLength = null;
                    if (_handle.Length >= HEADER_LENGTH)
                    {
                        ProcessPacket();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Exception(e);
            }
        }

        private void Handle(IMessage message)
        {
            // Debug.Log(JsonConvert.SerializeObject(message));
            _onMessage?.Invoke(message);
        }

        public void Send(IMessage message)
        {
            lock (_sendQueue)
            {
                _sendQueue.Enqueue(message);
            }
        }

        public void Send(byte[] message)
        {
            lock (_sendQueue)
            {
                _sendQueue.Enqueue(message);
            }
        }
        
        private void WriteToStream(Stream stream, byte[] array)
        {
            stream.Write(array, 0, array.Length);
        }

        private void HandleDisconnect()
        {
            if (_disconnectHandled) return;
            
            _disconnectHandled = true;
            _onDisconnect?.Invoke(true);
        }
    }
}