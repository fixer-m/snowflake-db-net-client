using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Snowflake.Client.Tests
{
    [TestFixture]
    class SnowflakeTypesConverterTest
    {
        [Test]
        [TestCase("2100-12-31 23:59:59.9999999")]
        [TestCase("2200-01-01 11:22:33.4455667")]
        [TestCase("9999-12-31 23:59:59.9999999")]
        [TestCase("1982-01-18 16:20:00.6666666")]
        [TestCase(null)]
        public void TestConvertDatetime(string inputTimeStr)
        {
            var inputTime = inputTimeStr == null ? DateTime.Now 
                : DateTime.ParseExact(inputTimeStr, "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture);

            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var tickDiff = inputTime.Ticks - unixEpoch.Ticks;
            var inputStringAsItWasFromDatabase = (tickDiff / 10000000.0m).ToString(CultureInfo.InvariantCulture);

            var result = SnowflakeTypesConverter.ConvertToDateTime(inputStringAsItWasFromDatabase, "timestamp_ntz");
            Assert.AreEqual(inputTime, result);
        }
    }
}
