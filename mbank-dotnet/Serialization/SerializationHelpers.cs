using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ib.mbank.Serialization
{
    internal static class SerializationHelpers
    {
        public static string ToBase64String<T>(this T objectToSerialize)
        {
            var formatter = new BinaryFormatter();

            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, objectToSerialize);
                return Convert.ToBase64String(ms.ToArray());
            }
        }
        public static T FromBase64String<T>(string base64String)
        {
            var formatter = new BinaryFormatter();

            using (var ms = new MemoryStream(Convert.FromBase64String(base64String)))
            {
                return (T)formatter.Deserialize(ms);

            }
        }
    }
}
