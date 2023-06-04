using System;
using System.Collections;
using System.IO;
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
            byte[] expectedUtf8Bytes = Encoding.UTF8.GetBytes(expectedBase64);

            byte[] actualUtf8Bytes;

            using (var ms = new MemoryStream(32))
            using (var sw = new StreamWriter(ms))
            {
                HexUtils.HexToBase64(hex, sw);
                sw.Flush();
                actualUtf8Bytes = ms.ToArray();
            }

            Assert.AreEqual(expectedUtf8Bytes, actualUtf8Bytes);
        }

        [Test]
        [TestCaseSource("TestCases")]
        public void ConvertHexToBytes_VaryingLengths(string hex, byte[] expectedBytes)
        {
            string expectedBase64 = Convert.ToBase64String(expectedBytes);
            byte[] expectedUtf8Bytes = Encoding.UTF8.GetBytes(expectedBase64);

            byte[] actualUtf8Bytes;

            using(var ms = new MemoryStream(32))
            using(var sw = new StreamWriter(ms))
            {
                HexUtils.HexToBase64(hex, sw);
                sw.Flush();
                actualUtf8Bytes = ms.ToArray();
            }

            Assert.AreEqual(expectedUtf8Bytes, actualUtf8Bytes);
        }

        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData("00", new byte[] { 0x00 });
                yield return new TestCaseData("0080", new byte[] { 0x00, 0x80 });
                yield return new TestCaseData("0080ff", new byte[] { 0x00, 0x80, 0xff });
                yield return new TestCaseData("0a0b0c0d", new byte[] { 0x0a, 0x0b, 0x0c, 0x0d });
                yield return new TestCaseData("0a0b0c0d0e", new byte[] { 0x0a, 0x0b, 0x0c, 0x0d, 0x0e });
                yield return new TestCaseData("0a0b0c0d0e0f", new byte[] { 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f });
            }
        }

    }
}
