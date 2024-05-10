using System.Text;

namespace JeeLee.UniNetworking.Messages.Payloads
{
    /// <summary>
    /// Interface for writing to a payload object.
    /// </summary>
    public interface IWriteablePayload
    {
        /// <summary>
        /// Writes a boolean to this payload.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void WriteBool(bool value);

        /// <summary>
        /// Writes a float to this payload.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void WriteFloat(float value);

        /// <summary>
        /// Writes an integer to this payload.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void WriteInt(int value);

        /// <summary>
        /// Writes a string to this payload.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="encoding">The encoding object to use for this string.</param>
        void WriteString(string value, Encoding encoding = null);
    }
}