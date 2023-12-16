using System.Text;

namespace JeeLee.Networking.Messages.Streams
{
    public interface IReadDataStream
    {
        bool ReadBool(bool movePointer = true);
        float ReadFloat(bool movePointer = true);
        int ReadInt(bool movePointer = true);
        string ReadString(Encoding encoding = null, bool movePointer = true);
    }
}