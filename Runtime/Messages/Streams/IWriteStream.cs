using System.Text;

namespace JeeLee.Networking.Messages.Streams
{
    public interface IWriteDataStream
    {
        void WriteBool(bool value);
        void WriteFloat(float value);
        void WriteInt(int value);
        void WriteString(string value, Encoding encoding = null);
    }
}