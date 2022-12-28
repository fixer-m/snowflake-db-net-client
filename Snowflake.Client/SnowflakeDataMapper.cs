using Snowflake.Client.Json;
using System;
using System.Collections.Generic;
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

            foreach (var rowRecord in rows)
            {
                var jsonString = BuildJsonString(columns, rowRecord);
                yield return JsonSerializer.Deserialize<T>(jsonString, _jsonMapperOptions);
            }
        }

        private static string BuildJsonString(List<ColumnDescription> columns, List<string> rowRecord)
        {
            var keyValuePairs = new List<string>();

            for (var i = 0; i < columns.Count; i++)
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

            switch (columnType)
            {
                case "text":
                    return JsonSerializer.Serialize(value);

                case "fixed":
                case "real":
                    return value;

                case "boolean":
                    return value == "1" || value.ToLower() == "true" ? "true" : "false";

                case "date":
                case "time":
                case "timestamp_ntz":
                    return '"' + SnowflakeTypesConverter.ConvertToDateTime(value, columnType).ToString("o") + '"';

                case "timestamp_ltz":
                case "timestamp_tz":
                    return '"' + SnowflakeTypesConverter.ConvertToDateTimeOffset(value, columnType).ToString("o") + '"';

                case "object":
                case "variant":
                case "array":
                    return value;

                case "binary":
                    var bytes = SnowflakeTypesConverter.HexToBytes(value);
                    var base64 = Convert.ToBase64String(bytes);
                    return '"' + base64 + '"';

                default:
                    return '"' + value + '"';
            }
        }
    }
}