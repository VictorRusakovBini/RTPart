using Google.Protobuf;

namespace core.Networking
{
    public class FastArray : INetArray
    {
        private const int LENGTH = 256;
        private byte[] _array;
        private int _position;

        public int Length => _position;

        public FastArray()
        {
            _array = new byte[LENGTH];
        }

        public void Write(byte [] data, int ? offset = null, int ? count = null)
        {
            if (count == null)
            {
                count = data.Length;
            }

            if (offset == null)
            {
                offset = 0;
            }

            if (_array.Length - _position < count)
            {
                var newSize = _array.Length * 2;
                if (_array.Length * 2 - _position < count)
                {
                    newSize = _array.Length + count.Value * 2;
                }

                Array.Resize(ref _array, newSize);
            }
            
            Array.Copy(data, offset.Value, _array, _position, count.Value);
            _position += count.Value;
        }

        public short ReadShort()
        {
            if (_position >= 2)
            {
                var value = BitConverter.ToInt16(_array);
                Array.Copy(_array,2, _array,0,_array.Length - 2);
                _position -= 2;
                return value;
            }

            return 0;
        }

        public IMessage GetData(MessageParser parser, short len)
        {
            var message = parser.ParseFrom(_array,0,len);

            if (len > 0)
            {
                Array.Copy(_array, len, _array, 0, _array.Length - len);
                _position -= len;
            }

            return message;
        }
    }
}