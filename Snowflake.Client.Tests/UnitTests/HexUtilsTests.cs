using System;
using System.Text;
using NUnit.Framework;
using Snowflake.Client.Helpers;

namespace Snowflake.Client.Tests.UnitTests
{
    [TestFixture]
    public class HexUtilsTests
    {
        [Test]
        [TestCase("0080ff", 0, 128, 255)]
        [TestCase("0a0b0c", 10, 11, 12)]
        public void ConvertHexToBytes(string hex, byte b1, byte b2, byte b3)
        {
            var expectedBytes = new byte[] { b1, b2, b3 };
            string expectedBase64 = Convert.ToBase64String(expectedBytes);

            var sb = new StringBuilder();
            HexUtils.HexToBase64(hex, sb);

            Assert.AreEqual(expectedBase64, sb.ToString());
        }
    }
}
