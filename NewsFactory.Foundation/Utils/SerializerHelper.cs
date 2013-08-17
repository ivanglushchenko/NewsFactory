using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace NewsFactory.Foundation.Utils
{
    public static class SerializerHelper
    {
        public static T Deserialize<T>(string json) where T : new()
        {
            if (string.IsNullOrEmpty(json)) return default(T);

            try
            {
                var _Bytes = Encoding.Unicode.GetBytes(json);
                using (var _Stream = new MemoryStream(_Bytes))
                {
                    var _Serializer = new DataContractJsonSerializer(typeof(T));
                    return (T)_Serializer.ReadObject(_Stream);
                }
            }
            catch
            {
            }

            return new T();
        }

        public static string Serialize(object instance)
        {
            using (var _Stream = new MemoryStream())
            {
                var _Serializer = new DataContractJsonSerializer(instance.GetType());
                _Serializer.WriteObject(_Stream, instance);
                _Stream.Position = 0;
                using (StreamReader _Reader = new StreamReader(_Stream))
                {
                    return _Reader.ReadToEnd();
                }
            }
        }
    }
}
