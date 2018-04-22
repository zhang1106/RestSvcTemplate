using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace AiDollar.ApiGateway.Http
{
    public class JsonNetFormatter : MediaTypeFormatter
    {
        private readonly JsonSerializer _serializer;

        public JsonNetFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));

            SupportedEncodings.Add(new UTF8Encoding(false));
            SupportedEncodings.Add(Encoding.Unicode);
            SupportedEncodings.Add(Encoding.ASCII);

            _serializer = new JsonSerializer();
            _serializer.Converters.Add(new IsoDateTimeConverter());
            _serializer.Converters.Add(new StringEnumConverter());
            _serializer.Formatting = Formatting.Indented;
        }

        public override bool CanWriteType(Type type)
        {
            // don't serialize JsonValue structure use default for that
            if (type == typeof(JValue) || type == typeof(JObject) || type == typeof(JArray))
                return false;

            return true;
        }

        public override bool CanReadType(Type type)
        {
            return true;
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream stream,
            HttpContent content, IFormatterLogger formatterLogger)
        {
            var encoding = SelectCharacterEncoding(content.Headers);
            using (var sr = new StreamReader(stream, encoding, false, 4096, true))
            using (var reader = new JsonTextReader(sr))
            {
                return Task.FromResult(_serializer.Deserialize(reader, type));
            }
        }

        public override async Task WriteToStreamAsync(Type type, object value, Stream stream,
            HttpContent content, TransportContext transportContext)
        {
            var encoding = SelectCharacterEncoding(content.Headers);
            using (var sw = new StreamWriter(stream, encoding, 4096, true))
            using (var writer = new JsonTextWriter(sw))
            {
                IEnumerable array;
                if (value != null && value.GetType() != typeof(string) && (array = value as IEnumerable) != null)
                {
                    writer.WriteStartArray();
                    foreach (var item in array)
                    {
                        _serializer.Serialize(writer, item);
                    }
                    writer.WriteEndArray();
                }
                else
                {
                    _serializer.Serialize(writer, value);
                }

                await sw.FlushAsync();
            }
        }
    }
}
