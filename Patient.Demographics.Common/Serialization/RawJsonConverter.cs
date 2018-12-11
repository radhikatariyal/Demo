using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Patient.Demographics.Common.Serialization
{
    public class RawJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var json = value.ToString();
            writer.WriteRawValue(json);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(string).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jobject = JObject.Load(reader);
            var json = jobject.ToString(Formatting.None);
            return json;
        }

        public override bool CanRead
        {
            get { return true; }
        }
    }
}