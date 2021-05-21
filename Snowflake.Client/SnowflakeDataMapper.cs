using Snowflake.Client.Json;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Snowflake.Client
{
    public static class SnowflakeDataMapper
    {
        private static JsonSerializerOptions JsonMapperOptions = new JsonSerializerOptions();

        public static void SetJsonMapperOptions(JsonSerializerOptions jsonMapperOptions)
        {
            if (jsonMapperOptions != null)
                JsonMapperOptions = jsonMapperOptions;
        }

        public static IEnumerable<T> MapTo<T>(List<ColumnDescription> columns, List<List<string>> rows)
        {
            if (columns == null || columns.Count == 0)
                throw new ArgumentNullException("Columns argument should be not empty.");

            if (rows == null)
                throw new ArgumentNullException(nameof(rows));

            foreach (var rowRecord in rows)
            {
                var jsonString = BuildJsonString(columns, rowRecord);
                yield return JsonSerializer.Deserialize<T>(jsonString, JsonMapperOptions);
            }
        }

        private static string BuildJsonString(List<ColumnDescription> columns, List<string> rowRecord)
        {
            var keyValuePairs = new List<string>();

            for (int i = 0; i < columns.Count; i++)
            {
                var jsonTokenValue = ConvertColumnValueToJsonToken(rowRecord[i], columns[i].Type);
                var kvPair = $"\"{columns[i].Name}\": {jsonTokenValue}";
                keyValuePairs.Add(kvPair);
            }

            var allKeyPairsJoined = string.Join(", ", keyValuePairs);
            var json = "{ " + allKeyPairsJoined + " }";

            return json;
        }

        private static string ConvertColumnValueToJsonToken(string value, string columnType)
        {
            if (value == null || value == "null")
                return "null";

            if (columnType == "text")
                return JsonSerializer.Serialize(value);

            var sfSemiStructuredTypes = new List<string>() { "object", "variant", "array" };
            if (sfSemiStructuredTypes.Contains(columnType))
                return value;

            var sfSimpleTypes = new List<string>() { "fixed", "real" };
            if (sfSimpleTypes.Contains(columnType))
                return value;

            if (columnType == "boolean")
                return value == "1" || value.ToLower() == "true" ? "true" : "false";

            var sfDateTimeOffsetTypes = new List<string>() { "timestamp_ltz", "timestamp_tz" };
            if (sfDateTimeOffsetTypes.Contains(columnType))
                return '"' + SnowflakeTypesConverter.ConvertToDateTimeOffset(value, columnType).ToString("o") + '"';

            var sfDateTimeTypes = new List<string>() { "date", "time", "timestamp_ntz" };
            if (sfDateTimeTypes.Contains(columnType))
                return '"' + SnowflakeTypesConverter.ConvertToDateTime(value, columnType).ToString("o") + '"';

            if (columnType == "binary")
            {
                var bytes = SnowflakeTypesConverter.HexToBytes(value);
                var base64 = Convert.ToBase64String(bytes);
                return '"' + base64 + '"';
            }

            return '"' + value + '"';
        }
    }
}