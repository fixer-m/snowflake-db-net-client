using NUnit.Framework;
using System;
using System.Globalization;

namespace Snowflake.Client.Tests.UnitTests
{
    [TestFixture]
    public class SnowflakeTypesConverterTest
    {
        [Test]
        [TestCase("4133980799.999999900", "2100-12-31 23:59:59.9999999")]
        [TestCase("7258159353.445566700", "2200-01-01 11:22:33.4455667")]
        [TestCase("253402300799.999999900", "9999-12-31 23:59:59.9999999")]
        [TestCase("380218800.666666600", "1982-01-18 16:20:00.6666666")]
        public void SnowflakeTimestampNtz_ToDateTime(string sfTimestampNtz, string expectedDateTimeString)
        {
            var expectedDateTime = DateTime.ParseExact(expectedDateTimeString, "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
            var convertedResult = SnowflakeTypesConverter.ConvertToDateTime(sfTimestampNtz, "timestamp_ntz");

            Assert.AreEqual(expectedDateTime, convertedResult);
        }

        [Test]
        [TestCase("4133980799.999999900", "2100-12-31 23:59:59.9999999")]
        [TestCase("7258159353.445566700", "2200-01-01 11:22:33.4455667")]
        [TestCase("253402300799.999999900", "9999-12-31 23:59:59.9999999")]
        [TestCase("380218800.666666600", "1982-01-18 16:20:00.6666666")]
        public void SnowflakeTime_ToDateTime(string sfTime, string expectedDateTimeString)
        {
            var expectedDateTime = DateTime.ParseExact(expectedDateTimeString, "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
            var convertedResult = SnowflakeTypesConverter.ConvertToDateTime(sfTime, "time"); // same conversion as for "timestamp_ntz"

            Assert.AreEqual(expectedDateTime, convertedResult);
        }

        [Test]
        [TestCase("47846", "2100-12-31")]
        [TestCase("84006", "2200-01-01")]
        [TestCase("2932896", "9999-12-31")]
        [TestCase("4400", "1982-01-18")]
        public void SnowflakeDate_ToDateTime(string sfDate, string expectedDateTimeString)
        {
            var expectedDateTime = DateTime.ParseExact(expectedDateTimeString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var convertedResult = SnowflakeTypesConverter.ConvertToDateTime(sfDate, "date");

            Assert.AreEqual(expectedDateTime, convertedResult);
        }

        [Test]
        [TestCase("2100-12-31 23:59:59.9999999", "4133980799999999900")]
        [TestCase("2200-01-01 11:22:33.4455667", "7258159353445566700")]
        [TestCase("9999-12-31 23:59:59.9999999", "253402300799999999900")]
        [TestCase("1982-01-18 16:20:00.6666666", "380218800666666600")]
        public void DateTime_ToSnowflakeTimestampNtz(string dateTimeString, string expectedSnowflakeTimestampNtz)
        {
            var dateTime = DateTime.ParseExact(dateTimeString, "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
            var convertedResult = SnowflakeTypesConverter.ConvertToTimestampNtz(dateTime);

            Assert.AreEqual(expectedSnowflakeTimestampNtz, convertedResult);
        }

        [Test]
        [TestCase("2100-12-31 23:59:59.9999999", "4133980799999999900 1440")]
        [TestCase("2200-01-01 11:22:33.4455667", "7258159353445566700 1440")]
        [TestCase("9999-12-31 23:59:59.9999999", "253402300799999999900 1440")]
        [TestCase("1982-01-18 16:20:00.6666666", "380218800666666600 1440")]
        public void DateTimeOffset_ToSnowflakeTimestampTz(string dateTimeString, string expectedSnowflakeTimestampTz)
        {
            var dateTimeOffsetUtc = DateTimeOffset.ParseExact(dateTimeString, "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            var convertedResult = SnowflakeTypesConverter.ConvertToTimestampTz(dateTimeOffsetUtc);

            Assert.AreEqual(expectedSnowflakeTimestampTz, convertedResult);
        }

        [Test]
        [TestCase(0, 128, 255, "0080ff")]
        [TestCase(10, 11, 12, "0a0b0c")]
        public void ConvertBytesToHex(byte b1, byte b2, byte b3, string hexExpected)
        {
            var hexResult = SnowflakeTypesConverter.BytesToHex(new byte[] { b1, b2, b3 });
            Assert.AreEqual(hexExpected, hexResult);
        }

        [Test]
        [TestCase("0080ff", 0, 128, 255)]
        [TestCase("0a0b0c", 10, 11, 12)]
        public void ConvertHexToBytes(string hex, byte b1, byte b2, byte b3)
        {
            var expectedBytes = new byte[] { b1, b2, b3 };
            var resultBytes = SnowflakeTypesConverter.HexToBytes(hex);

            Assert.AreEqual(expectedBytes, resultBytes);
        }
    }
}
