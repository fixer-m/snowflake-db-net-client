using Microsoft.Extensions.ObjectPool;
using Snowflake.Client.Helpers;
using Snowflake.Client.Json;
using Snowflake.Client.ObjectPool;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Snowflake.Client
{
    public static class SnowflakeDataMapper
    {
        private static readonly ObjectPool<StringBuilder> __stringBuilderPool =
            new DefaultObjectPool<StringBuilder>(
                new CustomStringBuilderPooledObjectPolicy());

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

            // Get a string builder from the pool.
            var sb = __stringBuilderPool.Get();
            string jsonToken;

            try
            {
                // Ensure the string builder is cleared before we use it.
                sb.Clear();
                ConvertColumnValueToJsonToken(value, column.Type, sb);
                jsonToken = sb.ToString();
            }
            finally
            {
                __stringBuilderPool.Return(sb);
            }

            return JsonSerializer.Deserialize<T>(jsonToken, _jsonMapperOptions);
        }

        public static IEnumerable<T> MapTo<T>(List<ColumnDescription> columns, List<List<string>> rows)
        {
            if (columns == null || columns.Count == 0)
                throw new ArgumentNullException(nameof(columns));

            if (rows == null)
                throw new ArgumentNullException(nameof(rows));

            // Get a string builder from the pool.
            var sb = __stringBuilderPool.Get();

            try
            {
                foreach (var rowRecord in rows)
                {
                    // Ensure the string builder is cleared before we use it.
                    sb.Clear();

                    BuildJsonString(columns, rowRecord, sb);
                    string jsonString = sb.ToString();
                    yield return JsonSerializer.Deserialize<T>(jsonString, _jsonMapperOptions);
                }
            }
            finally
            {
                __stringBuilderPool.Return(sb);
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
                    sb.Append('"');
                    HexUtils.HexToBase64(value, sb);
                    sb.Append('"');
                    break;

                default:
                    sb.Append(value);
                    break;
            }
        }
    }
}