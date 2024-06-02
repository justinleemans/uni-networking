using System.Text;

namespace JeeLee.UniNetworking.Payloads
{
    /// <summary>
    /// Interface for reading data from a payload object.
    /// </summary>
    public interface IReadablePayload
    {
        /// <summary>
        /// Reads a boolean value from this payload.
        /// </summary>
        /// <param name="movePointer">Should the pointer be moved after reading the value.</param>
        /// <returns>The boolean value read from the payload.</returns>
        bool ReadBool(bool movePointer = true);

        /// <summary>
        /// Reads a float value from this payload.
        /// </summary>
        /// <param name="movePointer">Should the pointer be moved after reading the value.</param>
        /// <returns>The flaot value read from the payload.</returns>
        float ReadFloat(bool movePointer = true);

        /// <summary>
        /// Reads a short value from this payload.
        /// </summary>
        /// <param name="movePointer">Should the pointer be moved after reading the value.</param>
        /// <returns>The short value read from the payload.</returns>
        short ReadShort(bool movePointer = true);

        /// <summary>
        /// Reads an integer value from this payload.
        /// </summary>
        /// <param name="movePointer">Should the pointer be moved after reading the value.</param>
        /// <returns>The integer value read from the payload.</returns>
        int ReadInt(bool movePointer = true);

        /// <summary>
        /// Reads a string value from this payload.
        /// </summary>
        /// <param name="encoding">The encoding object to use for this string.</param>
        /// <param name="movePointer">Should the pointer be moved after reading the value.</param>
        /// <returns>The string value read from the payload.</returns>
        string ReadString(Encoding encoding = null, bool movePointer = true);
    }
}