using System;
using System.Text;
using Snowflake.Client.Model;

namespace Snowflake.Client
{
    /// <summary>
    /// Based on https://github.com/snowflakedb/snowflake-connector-net/blob/master/Snowflake.Data/Core/SFDataConverter.cs
    /// </summary>
    internal static class SnowflakeTypesConverter
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        internal static DateTime ConvertToDateTime(string value, string snowflakeType)
        {
            switch (snowflakeType)
            {
                case "date":
                    var srcValLong = long.Parse(value);
                    return UnixEpoch.AddDays(srcValLong);

                case "time": // to timespan? https://github.com/snowflakedb/snowflake-connector-net/issues/327
                             // https://github.com/snowflakedb/snowflake-connector-net/commit/1fa03d92cdf6f7ae5720fdef8ecf25371f0f4c95
                case "timestamp_ntz":
                    var secAndNsecTuple = ExtractTimestamp(value);
                    var tickDiff = secAndNsecTuple.Item1 * 10000000L + secAndNsecTuple.Item2 / 100L;
                    return UnixEpoch.AddTicks(tickDiff);

                default:
                    throw new SnowflakeException($"Conversion from {snowflakeType} to DateTime is not supported.");
            }
        }

        internal static DateTimeOffset ConvertToDateTimeOffset(string value, string snowflakeType)
        {
            switch (snowflakeType)
            {
                case "timestamp_tz":
                    var spaceIndex = value.IndexOf(' ');
                    var offset = int.Parse(value.Substring(spaceIndex + 1, value.Length - spaceIndex - 1));
                    var offSetTimespan = new TimeSpan((offset - 1440) / 60, 0, 0);

                    var secAndNsecTzPair = ExtractTimestamp(value.Substring(0, spaceIndex));
                    var ticksTz = (secAndNsecTzPair.Item1 * 1000 * 1000 * 1000 + secAndNsecTzPair.Item2) / 100;
                    return new DateTimeOffset(UnixEpoch.Ticks + ticksTz, TimeSpan.Zero).ToOffset(offSetTimespan);

                case "timestamp_ltz":
                    var secAndNsecLtzPair = ExtractTimestamp(value);
                    var ticksLtz = (secAndNsecLtzPair.Item1 * 1000 * 1000 * 1000 + secAndNsecLtzPair.Item2) / 100;
                    return new DateTimeOffset(UnixEpoch.Ticks + ticksLtz, TimeSpan.Zero).ToLocalTime();

                default:
                    throw new SnowflakeException($"Conversion from {snowflakeType} to DateTimeOffset is not supported.");
            }
        }

        internal static string ConvertToTimestampTz(DateTimeOffset value)
        {
            var ticksPart = value.UtcTicks - UnixEpoch.Ticks;
            var minutesPart = value.Offset.TotalMinutes + 1440;

            return $"{ticksPart}00 {minutesPart}";
        }

        internal static string ConvertToTimestampNtz(DateTime value)
        {
            var diff = value.Subtract(UnixEpoch);

            return $"{diff.Ticks}00";
        }

        internal static string BytesToHex(byte[] bytes)
        {
            var hexBuilder = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                hexBuilder.AppendFormat("{0:x2}", b);
            }

            return hexBuilder.ToString();
        }

        private static Tuple<long, long> ExtractTimestamp(string srcVal)
        {
            var dotIndex = srcVal.IndexOf('.');

            if (dotIndex == -1)
                return Tuple.Create(long.Parse(srcVal), 0L);

            var intPart = long.Parse(srcVal.Substring(0, dotIndex));
            var decimalPartLength = srcVal.Length - dotIndex - 1;
            var decimalPartStr = srcVal.Substring(dotIndex + 1, decimalPartLength);
            var decimalPart = long.Parse(decimalPartStr);

            // If the decimal part contained less than nine characters, we must convert the value to nanoseconds by
            // multiplying by 10^[precision difference].
            if (decimalPartLength < 9)
            {
                decimalPart *= (int)Math.Pow(10, 9 - decimalPartLength);
            }

            return Tuple.Create(intPart, decimalPart);
        }
    }
}