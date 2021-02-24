using NUnit.Framework;
using Snowflake.Client.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Snowflake.Client.Tests.UnitTests
{
    [TestFixture]
    public class SnowflakeDataMapperTests
    {
        [Test]
        public void SnowflakeDataMapper_MapTo_CustomClass()
        {
            var responseSample = GetFakeResponse();

            var mappedObjects = SnowflakeDataMapper.MapTo<CustomClass>(responseSample.RowType, responseSample.RowSet);
            var firstObject = mappedObjects.FirstOrDefault();

            Assert.IsNotNull(firstObject);
            Assert.AreEqual("Sometext", firstObject.StringProperty);
            Assert.AreEqual(true, firstObject.BoolProperty);
            Assert.AreEqual(7, firstObject.IntProperty);
            Assert.AreEqual(27.6F, firstObject.FloatProperty);
            Assert.AreEqual(19.239834M, firstObject.DecimalProperty);
            Assert.AreEqual(Guid.Parse("e7412bbf-88ee-4149-b341-101e0f72ec7c"), firstObject.GuidProperty);
            Assert.AreEqual(new byte[] { 0, 128, 255 }, firstObject.ByteArrayProperty);

            var dateTime = DateTime.ParseExact("2020-09-13 12:26:40.0000000", "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
            Assert.AreEqual(dateTime, firstObject.DateTimeProperty);

            var dateTimeOffset = DateTimeOffset.ParseExact("2020-09-13 12:26:40.0000000", "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            Assert.AreEqual(dateTimeOffset, firstObject.DateTimeOffsetProperty);
        }

        [Test]
        public void SnowflakeDataMapper_MapTo_CustomClassWithNullableProps()
        {
            var responseSample = GetFakeResponse();

            var mappedObjects = SnowflakeDataMapper.MapTo<CustomClassNullables>(responseSample.RowType, responseSample.RowSet);
            var firstObject = mappedObjects.FirstOrDefault();

            Assert.IsNotNull(firstObject);
            Assert.AreEqual(true, firstObject.BoolProperty);
            Assert.AreEqual(7, firstObject.IntProperty);
            Assert.AreEqual(27.6F, firstObject.FloatProperty);
            Assert.AreEqual(19.239834M, firstObject.DecimalProperty);
            Assert.AreEqual(Guid.Parse("e7412bbf-88ee-4149-b341-101e0f72ec7c"), firstObject.GuidProperty);

            var dateTime = DateTime.ParseExact("2020-09-13 12:26:40.0000000", "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
            Assert.AreEqual(dateTime, firstObject.DateTimeProperty);

            var dateTimeOffset = DateTimeOffset.ParseExact("2020-09-13 12:26:40.0000000", "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            Assert.AreEqual(dateTimeOffset, firstObject.DateTimeOffsetProperty);
        }

        private QueryExecResponseData GetFakeResponse()
        {
            var response = new QueryExecResponseData() { RowType = new List<ColumnDescription>(), RowSet = new List<List<string>>() };

            response.RowType.Add(new ColumnDescription() { Name = "StringProperty", Type = "text" });
            response.RowType.Add(new ColumnDescription() { Name = "BoolProperty", Type = "boolean" });
            response.RowType.Add(new ColumnDescription() { Name = "IntProperty", Type = "fixed" });
            response.RowType.Add(new ColumnDescription() { Name = "FloatProperty", Type = "real" });
            response.RowType.Add(new ColumnDescription() { Name = "DecimalProperty", Type = "real" });
            response.RowType.Add(new ColumnDescription() { Name = "DateTimeProperty", Type = "timestamp_ntz" });
            response.RowType.Add(new ColumnDescription() { Name = "DateTimeOffsetProperty", Type = "timestamp_ltz" });
            response.RowType.Add(new ColumnDescription() { Name = "GuidProperty", Type = "text" });
            response.RowType.Add(new ColumnDescription() { Name = "ByteArrayProperty", Type = "binary" });

            response.RowSet.Add(new List<string>() {
                "Sometext", "true", "7", "27.6", "19.239834", "1600000000.000000000",
                "1600000000.000000000", "e7412bbf-88ee-4149-b341-101e0f72ec7c", "0080ff" });

            return response;
        }

        private class CustomClass
        {
            public string StringProperty { get; set; }

            public bool BoolProperty { get; set; }

            public int IntProperty { get; set; }

            public float FloatProperty { get; set; }

            public decimal DecimalProperty { get; set; }

            public DateTime DateTimeProperty { get; set; }

            public DateTimeOffset DateTimeOffsetProperty { get; set; }

            public Guid GuidProperty { get; set; }

            public byte[] ByteArrayProperty { get; set; }
        }

        private class CustomClassNullables
        {
            public bool? BoolProperty { get; set; }

            public int? IntProperty { get; set; }

            public float? FloatProperty { get; set; }

            public decimal? DecimalProperty { get; set; }

            public DateTime? DateTimeProperty { get; set; }

            public DateTimeOffset? DateTimeOffsetProperty { get; set; }

            public Guid? GuidProperty { get; set; }
        }
    }
}
