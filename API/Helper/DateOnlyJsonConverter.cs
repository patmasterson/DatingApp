using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Helper
{
    public class DateOnlyJsonConverter : JsonConverter<DateOnly?>
    {
        const string _format = "yyyy-mm-dd";

        public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            DateOnly doRetVal = DateOnly.ParseExact(reader.GetString()!, _format, CultureInfo.InvariantCulture);
            return doRetVal;
        }

        public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
        {
            string dateValue = "";
            if (value.HasValue)
                dateValue = value.ToString();

            writer.WriteStringValue(dateValue);
        }
    }
}