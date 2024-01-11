using System.Text;

namespace JeeLee.Networking.Messages.Streams
{
    /// <summary>
    /// Interface for reading data from a data stream object.
    /// </summary>
    public interface IReadDataStream
    {
        /// <summary>
        /// Reads a boolean value from this data stream.
        /// </summary>
        /// <param name="movePointer">Should the pointer be moved after reading the value.</param>
        /// <returns>The boolean value read from the data stream.</returns>
        bool ReadBool(bool movePointer = true);

        /// <summary>
        /// Reads a float value from this data stream.
        /// </summary>
        /// <param name="movePointer">Should the pointer be moved after reading the value.</param>
        /// <returns>The flaot value read from the data stream.</returns>
        float ReadFloat(bool movePointer = true);

        /// <summary>
        /// Reads an integer value from this data stream.
        /// </summary>
        /// <param name="movePointer">Should the pointer be moved after reading the value.</param>
        /// <returns>The integer value read from the data stream.</returns>
        int ReadInt(bool movePointer = true);

        /// <summary>
        /// Reads a string value from this data stream.
        /// </summary>
        /// <param name="encoding">The encoding object to use for this string.</param>
        /// <param name="movePointer">Should the pointer be moved after reading the value.</param>
        /// <returns>The string value read from the data stream.</returns>
        string ReadString(Encoding encoding = null, bool movePointer = true);
    }
}