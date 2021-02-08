using NUnit.Framework;
using System;
using System.Globalization;

namespace Snowflake.Client.Tests.UnitTests
{
    [TestFixture]
    public class ParameterBindingsSingleValuesTest
    {
        [Test]
        [TestCase(-42, typeof(int), "FIXED")]
        [TestCase(42, typeof(uint), "FIXED")]
        [TestCase(-42, typeof(sbyte), "FIXED")]
        [TestCase(42, typeof(byte), "FIXED")]
        [TestCase(-42, typeof(short), "FIXED")]
        [TestCase(42, typeof(ushort), "FIXED")]
        [TestCase(-42, typeof(long), "FIXED")]
        [TestCase(42, typeof(ulong), "FIXED")]
        [TestCase(42.5, typeof(float), "REAL")]
        [TestCase(-42.5, typeof(double), "REAL")]
        [TestCase(42.5, typeof(decimal), "REAL")]
        public void BuildParameters_Numeric(object objectValue, Type type, string bindingType)
        {
            var value = Convert.ChangeType(objectValue, type);
            var binding = ParameterBinder.BuildParameterBindings(value);

            Assert.AreEqual(1, binding.Count);
            Assert.IsTrue(binding.ContainsKey("1"));
            Assert.AreEqual(bindingType, binding["1"].Type);

            var stringValue = string.Format(CultureInfo.InvariantCulture, "{0}", objectValue);
            Assert.AreEqual(stringValue, binding["1"].Value);
        }

        [Test]
        [TestCase(true, "BOOLEAN")]
        [TestCase(false, "BOOLEAN")]
        public void BuildParameters_Bool(bool value, string bindingType)
        {
            var binding = ParameterBinder.BuildParameterBindings(value);

            Assert.AreEqual(1, binding.Count);
            Assert.IsTrue(binding.ContainsKey("1"));
            Assert.AreEqual(bindingType, binding["1"].Type);
            Assert.AreEqual(value.ToString(), binding["1"].Value);
        }

        [Test]
        [TestCase("sometext", "TEXT")]
        [TestCase("", "TEXT")]
        public void BuildParameters_String(string value, string bindingType)
        {
            var binding = ParameterBinder.BuildParameterBindings(value);

            Assert.AreEqual(1, binding.Count);
            Assert.IsTrue(binding.ContainsKey("1"));
            Assert.AreEqual(bindingType, binding["1"].Type);
            Assert.AreEqual(value, binding["1"].Value);
        }

        [Test]
        public void BuildParameters_Guid()
        {
            var guid = Guid.NewGuid();

            var binding = ParameterBinder.BuildParameterBindings(guid);

            var stringValue = string.Format(CultureInfo.InvariantCulture, "{0}", guid);

            Assert.AreEqual(1, binding.Count);
            Assert.IsTrue(binding.ContainsKey("1"));
            Assert.AreEqual("TEXT", binding["1"].Type);
            Assert.AreEqual(stringValue, binding["1"].Value);
        }

        [Test]
        [TestCase(new byte[] { 200, 201, 202 }, "c8c9ca", "BINARY")]
        [TestCase(new byte[] { 0 }, "00", "BINARY")]
        public void BuildParameters_BytesArray(byte[] value, string expectedString, string bindingType)
        {
            var binding = ParameterBinder.BuildParameterBindings(value);

            Assert.AreEqual(1, binding.Count);
            Assert.IsTrue(binding.ContainsKey("1"));
            Assert.AreEqual(bindingType, binding["1"].Type);
            Assert.AreEqual(expectedString, binding["1"].Value);
        }

        [Test]
        [TestCase("2021-06-10 16:17:18.0000000", "1623341838000000000", "TIMESTAMP_NTZ")]
        public void BuildParameters_DateTime(string stringValue, string expectedString, string bindingType)
        {
            var value = DateTime.ParseExact(stringValue, "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture);

            var binding = ParameterBinder.BuildParameterBindings(value);

            Assert.AreEqual(1, binding.Count);
            Assert.IsTrue(binding.ContainsKey("1"));
            Assert.AreEqual(bindingType, binding["1"].Type);
            Assert.AreEqual(expectedString, binding["1"].Value);
        }

        [Test]
        [TestCase("2021-06-10 16:17:18.0000000", "1623341838000000000 1440", "TIMESTAMP_TZ")]
        public void BuildParameters_DateTimeOffset(string stringValue, string expectedString, string bindingType)
        {
            var value = DateTimeOffset.ParseExact(stringValue, "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

            var binding = ParameterBinder.BuildParameterBindings(value);

            Assert.AreEqual(1, binding.Count);
            Assert.IsTrue(binding.ContainsKey("1"));
            Assert.AreEqual(bindingType, binding["1"].Type);
            Assert.AreEqual(expectedString, binding["1"].Value);
        }
    }
}
