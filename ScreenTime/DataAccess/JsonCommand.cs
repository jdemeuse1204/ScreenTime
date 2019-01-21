using System.IO;

namespace ScreenTime.DataAccess
{
    public class JsonCommand
    {
        public readonly string FileName;
        public readonly JsonConnection Connection;

        public JsonCommand(JsonConnection connection, string fileName)
        {
            FileName = fileName;
            Connection = connection;
        }

        public JsonDataReader GetReader()
        {
            return new JsonDataReader(Path.Combine(Connection.Path, FileName));
        }

        public JsonDataWriter GetWriter()
        {
            return new JsonDataWriter(Path.Combine(Connection.Path, FileName));
        }
    }
}
