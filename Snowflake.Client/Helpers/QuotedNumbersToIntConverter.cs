using System;
using System.Buffers;
using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Snowflake.Client.Helpers
{
    /// <summary>
    /// Allows to deserialize quoted numbers (i.e. strings) into int values
    /// </summary>
    internal class QuotedNumbersToIntConverter : JsonConverter<int?>
    {
        public override int? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                return reader.GetInt32();

            var span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
            if (Utf8Parser.TryParse(span, out int number, out var bytesConsumed) && span.Length == bytesConsumed)
                return number;

            return reader.GetInt32();
        }

        public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}