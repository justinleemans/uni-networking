using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JeeLee.Networking.Exceptions;

namespace JeeLee.Networking.Messages.Streams
{
    public class DataStream : IWriteDataStream, IReadDataStream
    {
        private List<byte> _buffer;
        private byte[] _readableBuffer;
        private int _pointer;

        public DataStream() : this(new List<byte>())
        {
        }

        public DataStream(IEnumerable<byte> dataBuffer)
        {
            _buffer = dataBuffer.ToList();
            _readableBuffer = _buffer.ToArray();
        }

        public byte[] GetBytes()
        {
            _readableBuffer = _buffer.ToArray();
            return _readableBuffer;
        }

        public void Sign(int messageId)
        {
            _buffer.InsertRange(0, BitConverter.GetBytes(messageId));
            _buffer.InsertRange(0, BitConverter.GetBytes(_buffer.Count));
        }

        public void Reset()
        {
            _buffer = new List<byte>();
            _readableBuffer = _buffer.ToArray();
            _pointer = 0;
        }

        #region IWriteDataStream Members

        public void WriteBool(bool value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void WriteFloat(float value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void WriteInt(int value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void WriteString(string value, Encoding encoding = null)
        {
            WriteInt(value.Length);
            _buffer.AddRange((encoding ?? Encoding.ASCII).GetBytes(value));
        }

        #endregion

        #region IReadDataStream Members

        public bool ReadBool(bool movePointer = true)
        {
            if (_pointer > _buffer.Count)
            {
                throw new DataStreamException();
            }

            bool value = BitConverter.ToBoolean(_readableBuffer, _pointer);

            if (movePointer)
            {
                _pointer += sizeof(bool);
            }

            return value;
        }

        public float ReadFloat(bool movePointer = true)
        {
            if (_pointer > _buffer.Count)
            {
                throw new DataStreamException();
            }

            float value = BitConverter.ToSingle(_readableBuffer, _pointer);

            if (movePointer)
            {
                _pointer += sizeof(float);
            }

            return value;
        }

        public int ReadInt(bool movePointer = true)
        {
            if (_pointer > _buffer.Count)
            {
                throw new DataStreamException();
            }

            int value = BitConverter.ToInt32(_readableBuffer, _pointer);

            if (movePointer)
            {
                _pointer += sizeof(int);
            }

            return value;
        }

        public string ReadString(Encoding encoding = null, bool movePointer = true)
        {
            try
            {
                int length = ReadInt();
                string value = (encoding ?? Encoding.ASCII).GetString(_readableBuffer, _pointer, length);

                if (movePointer && value.Length > 0)
                {
                    _pointer += length;
                }

                return value;
            }
            catch
            {
                throw new DataStreamException();
            }
        }

        #endregion
    }
}