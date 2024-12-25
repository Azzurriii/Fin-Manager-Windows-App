using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Fin_Manager_v2.Converters
{
    public class CustomDecimalConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    {
                        string? stringValue = reader.GetString();
                        if (decimal.TryParse(stringValue, out decimal result))
                        {
                            return result;
                        }
                        break;
                    }
                case JsonTokenType.Number:
                    return reader.GetDecimal();
            }

            throw new JsonException($"Unable to convert \"{reader.GetString()}\" to Decimal.");
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
