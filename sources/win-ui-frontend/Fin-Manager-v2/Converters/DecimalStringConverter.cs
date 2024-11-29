using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fin_Manager_v2.Converters
{
    public class DecimalStringConverter : JsonConverter<decimal?>
    {
        public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    var stringValue = reader.GetString();
                    if (string.IsNullOrWhiteSpace(stringValue))
                        return null;

                    if (decimal.TryParse(stringValue,
                        NumberStyles.Number |
                        NumberStyles.AllowDecimalPoint |
                        NumberStyles.AllowThousands,
                        CultureInfo.InvariantCulture,
                        out decimal decimalValue))
                    {
                        return decimalValue;
                    }

                    Console.WriteLine($"Could not parse decimal from string: {stringValue}");
                    return null;

                case JsonTokenType.Number:
                    return reader.GetDecimal();

                case JsonTokenType.Null:
                    return null;

                default:
                    throw new JsonException($"Unexpected token type: {reader.TokenType}");
            }
        }

        public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteNumberValue(value.Value);
            else
                writer.WriteNullValue();
        }
    }
}
