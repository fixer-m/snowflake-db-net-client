using NUnit.Framework;
using System;
using System.Globalization;

namespace Snowflake.Client.Tests.UnitTests
{
    [TestFixture]
    class SnowflakeTypesConverterTest
    {
        [Test]
        [TestCase("4133980799.9999999", "2100-12-31 23:59:59.9999999")]
        [TestCase("7258159353.4455667", "2200-01-01 11:22:33.4455667")]
        [TestCase("253402300799.9999999", "9999-12-31 23:59:59.9999999")]
        [TestCase("380218800.6666666", "1982-01-18 16:20:00.6666666")]
        public void ConvertSnowflakeTimestampNtzToDateTime(string sfTimestampNtz, string expectedDateTimeString)
        {
            var expectedDateTime = DateTime.ParseExact(expectedDateTimeString, "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
            var convertedResult = SnowflakeTypesConverter.ConvertToDateTime(sfTimestampNtz, "timestamp_ntz");

            Assert.AreEqual(convertedResult, expectedDateTime);
        }

        [Test]
        [TestCase("4133980799.9999999", "2100-12-31 23:59:59.9999999")]
        [TestCase("7258159353.4455667", "2200-01-01 11:22:33.4455667")]
        [TestCase("253402300799.9999999", "9999-12-31 23:59:59.9999999")]
        [TestCase("380218800.6666666", "1982-01-18 16:20:00.6666666")]
        public void ConvertSnowflakeTimeToDateTime(string sfTime, string expectedDateTimeString)
        {
            var expectedDateTime = DateTime.ParseExact(expectedDateTimeString, "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
            var convertedResult = SnowflakeTypesConverter.ConvertToDateTime(sfTime, "time");

            Assert.AreEqual(convertedResult, expectedDateTime);
        }

        [Test]
        [TestCase("1982-01-18 16:20:00.6666666", "380218800666666600")] // wtf?
        public void ConvertDateToSnowflakeTimestampNtz(string dateTimeString, string expectedSnowflakeTimestampNtz)
        {
            var dateTime = DateTime.ParseExact(dateTimeString, "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
            var convertedResult = SnowflakeTypesConverter.ConvertToTimestampNtz(dateTime);

            Assert.AreEqual(convertedResult, expectedSnowflakeTimestampNtz);
        }
    }
}
