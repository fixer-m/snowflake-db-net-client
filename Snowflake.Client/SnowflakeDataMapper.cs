using Snowflake.Client.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Snowflake.Client
{
    public static class SnowflakeDataMapper
    {
        private static JsonSerializerOptions _jsonMapperOptions = new JsonSerializerOptions();

        public static void Configure(JsonSerializerOptions jsonMapperOptions)
        {
            if (jsonMapperOptions != null)
                _jsonMapperOptions = jsonMapperOptions;
        }

        [Obsolete("Please use Configure method instead")]
        public static void SetJsonMapperOptions(JsonSerializerOptions jsonMapperOptions)
        {
            Configure(jsonMapperOptions);
        }

        public static T MapTo<T>(ColumnDescription column, string value)
        {
            if (column == null)
                throw new ArgumentNullException(nameof(column));

            var jsonToken = ConvertColumnValueToJsonToken(value, column.Type);
            return JsonSerializer.Deserialize<T>(jsonToken, _jsonMapperOptions);
        }

        public static IEnumerable<T> MapTo<T>(List<ColumnDescription> columns, List<List<string>> rows)
        {
            if (columns == null || columns.Count == 0)
                throw new ArgumentNullException(nameof(columns));

            if (rows == null)
                throw new ArgumentNullException(nameof(rows));

            // Create a string builder to be re-used for each record/row; this approach will minimise string allocations.
            var sb = new StringBuilder();

            foreach (var rowRecord in rows)
            {
                BuildJsonString(columns, rowRecord, sb);
                string jsonString = sb.ToString();
                yield return JsonSerializer.Deserialize<T>(jsonString, _jsonMapperOptions);

                // Clear string builder so that it can be re-used in the next loop, thus re-using all of its allocated memory.
                sb.Clear();
            }
        }

        private static void BuildJsonString(List<ColumnDescription> columns, List<string> rowRecord, StringBuilder sb)
        {
            // Append json opening brace.
            sb.Append('{');

            if (columns.Count != 0)
            {
                // Append first property.
                AppendAsJsonProperty(sb, columns[0].Name, rowRecord[0], columns[0].Type);

                // Append all other properties, prefixed with a comma to separate from previous property.
                for (int i = 1; i < columns.Count; i++)
                {
                    sb.Append(",");
                    AppendAsJsonProperty(sb, columns[i].Name, rowRecord[i], columns[i].Type);
                }
            }

            // Append json closing brace.
            sb.Append('}');
        }

        private static void AppendAsJsonProperty(
            StringBuilder sb,
            string propertyName,
            string columnValue,
            string columnType)
        {
            // Append property name and colon separator.
            sb.Append('"');
            sb.Append(propertyName);
            sb.Append("\":");

            // Append json property value.
            ConvertColumnValueToJsonToken(columnValue, columnType, sb);
        }

        private static string ConvertColumnValueToJsonToken(
            string value,
            string columnType)
        {
            var sb = new StringBuilder();
            ConvertColumnValueToJsonToken(value,columnType, sb);
            return sb.ToString();
        }

        private static void ConvertColumnValueToJsonToken(
            string value,
            string columnType,
            StringBuilder sb)
        {
            if(value is null || value == "null")
            {
                sb.Append("null");
                return;
            }

            switch(columnType)
            {
                case "text":
                    sb.Append(JsonSerializer.Serialize(value));
                    break;

                case "fixed":
                case "real":
                    sb.Append(value);
                    break;

                case "boolean":
                    sb.Append(value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase) ? "true" : "false");
                    break;

                case "date":
                case "time":
                case "timestamp_ntz":
                    sb.Append('"');
                    sb.Append(SnowflakeTypesConverter.ConvertToDateTime(value, columnType).ToString("o"));
                    sb.Append('"');
                    break;

                case "timestamp_ltz":
                case "timestamp_tz":
                    sb.Append('"');
                    sb.Append(SnowflakeTypesConverter.ConvertToDateTimeOffset(value, columnType).ToString("o"));
                    sb.Append('"');
                    break;

                case "object":
                case "variant":
                case "array":
                    sb.Append(value);
                    break;

                case "binary":
                    var bytes = SnowflakeTypesConverter.HexToBytes(value);
                    var base64 = Convert.ToBase64String(bytes);
                    sb.Append('"');
                    sb.Append(base64);
                    sb.Append('"');
                    break;

                default:
                    sb.Append(value);
                    break;
            }
        }
    }
}