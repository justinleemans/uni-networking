using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JeeLee.UniNetworking.Messages.Payloads
{
    /// <summary>
    /// The data container used to translate network messages.
    /// </summary>
    public sealed class Payload : IWriteablePayload, IReadablePayload
    {
        private List<byte> _buffer;
        private byte[] _readableBuffer;
        private int _pointer;

        /// <summary>
        /// The type of payload.
        /// </summary>
        public PayloadType Type { get; }

        /// <summary>
        /// The id of the message this payload relates to.
        /// </summary>
        public int MessageId { get; }

        /// <summary>
        /// Constructor to create a new payload object with the specified payload type.
        /// </summary>
        /// <param name="type">The type of the payload.</param>
        public Payload(PayloadType type) : this(new List<byte>())
        {
            Type = type;
        }

        /// <summary>
        /// Constructor to create a new payload object with the specified message identifier and payload type.
        /// </summary>
        /// <param name="messageId">The message identifier associated with the payload.</param>
        /// <param name="type">The type of the payload.</param>
        public Payload(int messageId, PayloadType type = default) : this(new List<byte>())
        {
            Type = type;
            MessageId = messageId;
        }

        /// <summary>
        /// Constructor to create a new payload object using the given data buffer.
        /// </summary>
        /// <param name="dataBuffer">The data to insert into this payload object upon creation.</param>
        public Payload(IEnumerable<byte> dataBuffer)
        {
            _buffer = dataBuffer.ToList();
            _readableBuffer = _buffer.ToArray();

            if (_buffer.Count <= 0)
            {
                return;
            }

            Type = (PayloadType)ReadInt();

            if (Type == PayloadType.Message)
            {
                MessageId = ReadInt();
            }
        }

        #region IWriteablePayload Members

        /// <summary>
        /// Writes a boolean to this payload.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteBool(bool value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Writes a float to this payload.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteFloat(float value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Writes an integer to this payload.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteInt(int value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Writes a string to this payload.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="encoding">The encoding object to use for this string.</param>
        public void WriteString(string value, Encoding encoding = null)
        {
            WriteInt(value.Length);
            _buffer.AddRange((encoding ?? Encoding.ASCII).GetBytes(value));
        }

        #endregion

        #region IReadablePayload Members

        /// <summary>
        /// Reads a boolean value from this payload.
        /// </summary>
        /// <param name="movePointer">Should the pointer be moved after reading the value.</param>
        /// <returns>The boolean value read from the payload.</returns>
        public bool ReadBool(bool movePointer = true)
        {
            if (_pointer > _buffer.Count)
            {
                throw new Exception("Pointer overflow, trying to read data(bool) past end of buffer");
            }

            bool value = BitConverter.ToBoolean(_readableBuffer, _pointer);

            if (movePointer)
            {
                _pointer += sizeof(bool);
            }

            return value;
        }

        /// <summary>
        /// Reads a float value from this payload.
        /// </summary>
        /// <param name="movePointer">Should the pointer be moved after reading the value.</param>
        /// <returns>The flaot value read from the payload.</returns>
        public float ReadFloat(bool movePointer = true)
        {
            if (_pointer > _buffer.Count)
            {
                throw new Exception("Pointer overflow, trying to read data(float) past end of buffer");
            }

            float value = BitConverter.ToSingle(_readableBuffer, _pointer);

            if (movePointer)
            {
                _pointer += sizeof(float);
            }

            return value;
        }

        /// <summary>
        /// Reads an integer value from this payload.
        /// </summary>
        /// <param name="movePointer">Should the pointer be moved after reading the value.</param>
        /// <returns>The integer value read from the payload.</returns>
        public int ReadInt(bool movePointer = true)
        {
            if (_pointer > _buffer.Count)
            {
                throw new Exception("Pointer overflow, trying to read data(int) past end of buffer");
            }

            int value = BitConverter.ToInt32(_readableBuffer, _pointer);

            if (movePointer)
            {
                _pointer += sizeof(int);
            }

            return value;
        }

        /// <summary>
        /// Reads a string value from this payload.
        /// </summary>
        /// <param name="encoding">The encoding object to use for this string.</param>
        /// <param name="movePointer">Should the pointer be moved after reading the value.</param>
        /// <returns>The string value read from the payload.</returns>
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
                throw new Exception("Error trying to read string data from buffer");
            }
        }

        #endregion

        /// <summary>
        /// Gets the bytes form this payload object.
        /// </summary>
        /// <returns>A byte array representation of the payload.</returns>
        public byte[] GetBytes()
        {
            SignPayload();
            
            _readableBuffer = _buffer.ToArray();
            return _readableBuffer;
        }

        /// <summary>
        /// Resets this payload object to a neutral and reusable state.
        /// </summary>
        public void Reset()
        {
            _buffer = new List<byte>();
            _readableBuffer = _buffer.ToArray();
            _pointer = 0;
        }

        private void SignPayload()
        {
            switch (Type)
            {
                case PayloadType.Message:
                _buffer.InsertRange(0, BitConverter.GetBytes(MessageId));
                    break;
            }

            _buffer.InsertRange(0, BitConverter.GetBytes((int)Type));
            _buffer.InsertRange(0, BitConverter.GetBytes(_buffer.Count));
        }
    }
}