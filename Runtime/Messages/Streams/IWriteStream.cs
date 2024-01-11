using System.Text;

namespace JeeLee.Networking.Messages.Streams
{
    /// <summary>
    /// Interface for writing to a data stream object.
    /// </summary>
    public interface IWriteDataStream
    {
        /// <summary>
        /// Writes a boolean to this data stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void WriteBool(bool value);

        /// <summary>
        /// Writes a float to this data stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void WriteFloat(float value);

        /// <summary>
        /// Writes an integer to this data stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void WriteInt(int value);

        /// <summary>
        /// Writes a string to this data stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="encoding">The encoding object to use for this string.</param>
        void WriteString(string value, Encoding encoding = null);
    }
}