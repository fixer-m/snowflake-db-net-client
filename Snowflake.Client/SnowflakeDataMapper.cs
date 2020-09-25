using Snowflake.Client.Json;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Snowflake.Client
{
    public static class SnowflakeDataMapper
    {
        public static IEnumerable<T> Map<T>(QueryExecResponseData data)
        {
            var columns = data.RowType;
            var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

            foreach (var rowRecord in data.RowSet)
            {
                var jsonString = BuildJsonString(columns, rowRecord);
                yield return JsonSerializer.Deserialize<T>(jsonString, jsonSerializerOptions);
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
            // todo: strings with a " (quote)
            if (value == null || value == "null")
                return "null";

            // todo: escape special chars in json (like 0A = LF - line feed)
            var sfTextTypes = new List<string>() { "text", "object", "variant", "array" };
            if (sfTextTypes.Contains(columnType))
                return '"' + value + '"';

            var sfSimpleTypes = new List<string>() { "fixed", "real" };
            if (sfSimpleTypes.Contains(columnType))
                return value;

            if (columnType == "boolean")
                return value == "1" || value == "true" ? "true" : "false";

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