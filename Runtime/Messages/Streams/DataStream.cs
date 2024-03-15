using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JeeLee.UniNetworking.Exceptions;

namespace JeeLee.UniNetworking.Messages.Streams
{
    /// <summary>
    /// The data container used to translate network messages.
    /// </summary>
    public sealed class DataStream : IWriteDataStream, IReadDataStream
    {
        private List<byte> _buffer;
        private byte[] _readableBuffer;
        private int _pointer;

        public bool IsWritten => _buffer.Any();

        /// <summary>
        /// Constructor to create a new empty data stream object.
        /// </summary>
        public DataStream() : this(new List<byte>())
        {
        }

        /// <summary>
        /// Constructor to create a new data stream object using the given data buffer.
        /// </summary>
        /// <param name="dataBuffer">The data to insert into this data stream object upon creation.</param>
        public DataStream(IEnumerable<byte> dataBuffer)
        {
            _buffer = dataBuffer.ToList();
            _readableBuffer = _buffer.ToArray();
        }

        #region IWriteDataStream Members

        /// <summary>
        /// Writes a boolean to this data stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteBool(bool value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Writes a float to this data stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteFloat(float value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Writes an integer to this data stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteInt(int value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Writes a string to this data stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="encoding">The encoding object to use for this string.</param>
        public void WriteString(string value, Encoding encoding = null)
        {
            WriteInt(value.Length);
            _buffer.AddRange((encoding ?? Encoding.ASCII).GetBytes(value));
        }

        #endregion

        #region IReadDataStream Members

        /// <summary>
        /// Reads a boolean value from this data stream.
        /// </summary>
        /// <param name="movePointer">Should the pointer be moved after reading the value.</param>
        /// <returns>The boolean value read from the data stream.</returns>
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

        /// <summary>
        /// Reads a float value from this data stream.
        /// </summary>
        /// <param name="movePointer">Should the pointer be moved after reading the value.</param>
        /// <returns>The flaot value read from the data stream.</returns>
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

        /// <summary>
        /// Reads an integer value from this data stream.
        /// </summary>
        /// <param name="movePointer">Should the pointer be moved after reading the value.</param>
        /// <returns>The integer value read from the data stream.</returns>
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

        /// <summary>
        /// Reads a string value from this data stream.
        /// </summary>
        /// <param name="encoding">The encoding object to use for this string.</param>
        /// <param name="movePointer">Should the pointer be moved after reading the value.</param>
        /// <returns>The string value read from the data stream.</returns>
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

        /// <summary>
        /// Gets the bytes form this data stream object.
        /// </summary>
        /// <returns>A byte array representation of the data stream.</returns>
        public byte[] GetBytes()
        {
            _readableBuffer = _buffer.ToArray();
            return _readableBuffer;
        }

        /// <summary>
        /// Signs this data stream with a prefixed signature.
        /// This signature consists of first the total message length without this length value itself and the message id.
        /// </summary>
        /// <param name="messageId"></param>
        public void Sign(int messageId)
        {
            _buffer.InsertRange(0, BitConverter.GetBytes(messageId));
            _buffer.InsertRange(0, BitConverter.GetBytes(_buffer.Count));
        }

        /// <summary>
        /// Resets this data stream object to a neutral and reusable state.
        /// </summary>
        public void Reset()
        {
            _buffer = new List<byte>();
            _readableBuffer = _buffer.ToArray();
            _pointer = 0;
        }
    }
}