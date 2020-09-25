using System;
using System.Text;

namespace Snowflake.Client
{
    /// <summary>
    /// Based on https://github.com/snowflakedb/snowflake-connector-net/blob/master/Snowflake.Data/Core/SFDataConverter.cs
    /// </summary>
    public static class SnowflakeTypesConverter
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ConvertToDateTime(string value, string snowflakeType)
        {
            switch (snowflakeType)
            {
                case "date":
                    long srcValLong = long.Parse(value);
                    return UnixEpoch.AddDays(srcValLong);

                case "time":
                case "timestamp_ntz":
                    var secAndNsecTuple = ExtractTimestamp(value);
                    var tickDiff = secAndNsecTuple.Item1 * 10000000L + secAndNsecTuple.Item2 / 100L;
                    return UnixEpoch.AddTicks(tickDiff);

                default:
                    throw new SnowflakeException($"Conversion from {snowflakeType} to DateTime is not supported.");
            }
        }

        public static DateTimeOffset ConvertToDateTimeOffset(string value, string snowflakeType)
        {
            switch (snowflakeType)
            {
                case "timestamp_tz":
                    int spaceIndex = value.IndexOf(' ');
                    int offset = int.Parse(value.Substring(spaceIndex + 1, value.Length - spaceIndex - 1));
                    TimeSpan offSetTimespan = new TimeSpan((offset - 1440) / 60, 0, 0);

                    var secAndNsecTZPair = ExtractTimestamp(value.Substring(0, spaceIndex));
                    long ticksTZ = (secAndNsecTZPair.Item1 * 1000 * 1000 * 1000 + secAndNsecTZPair.Item2) / 100;
                    return new DateTimeOffset(UnixEpoch.Ticks + ticksTZ, TimeSpan.Zero).ToOffset(offSetTimespan);

                case "timestamp_ltz":
                    var secAndNsecLTZPair = ExtractTimestamp(value);
                    long ticksLTZ = (secAndNsecLTZPair.Item1 * 1000 * 1000 * 1000 + secAndNsecLTZPair.Item2) / 100;
                    return new DateTimeOffset(UnixEpoch.Ticks + ticksLTZ, TimeSpan.Zero).ToLocalTime();

                default:
                    throw new SnowflakeException($"Conversion from {snowflakeType} to DateTimeOffset is not supported.");
            }
        }

        public static string ConvertToTimestampTz(DateTimeOffset value)
        {
            var ticksPart = (value.UtcTicks - UnixEpoch.Ticks) * 100L;
            var minutesPart = value.Offset.TotalMinutes + 1440;
            return $"{ticksPart} {minutesPart}";
        }

        public static string ConvertToTimestampNtz(DateTime value)
        {
            var diff = value.Subtract(UnixEpoch);
            return $"{diff.Ticks}00";
        }

        public static string BytesToHex(byte[] bytes)
        {
            var hexBuilder = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                hexBuilder.AppendFormat("{0:x2}", b);
            }
            return hexBuilder.ToString();
        }

        public static byte[] HexToBytes(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];

            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);

            return bytes;
        }

        private static Tuple<long, long> ExtractTimestamp(string srcVal)
        {
            int dotIndex = srcVal.IndexOf('.');

            if (dotIndex == -1)
                return Tuple.Create(long.Parse(srcVal), (long)0);

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