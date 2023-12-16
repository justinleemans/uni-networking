using System;

namespace JeeLee.Networking.Exceptions
{
    public class DataStreamException : Exception
    {
        public DataStreamException()
        {
        }

        public DataStreamException(string message) : base(message)
        {
        }

        public DataStreamException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}