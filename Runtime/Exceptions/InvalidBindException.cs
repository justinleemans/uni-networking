using System;

namespace JeeLee.UniNetworking.Exceptions
{
    public class InvalidBindException : Exception
    {
        public InvalidBindException()
        {
        }

        public InvalidBindException(string message) : base(message)
        {
        }

        public InvalidBindException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}