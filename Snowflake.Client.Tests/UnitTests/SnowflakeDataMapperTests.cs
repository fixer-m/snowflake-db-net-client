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
        public void ResponseWithValues_MapTo_CustomClass()
        {
            var responseSample = GetFakeResponse();
            var firstObjectRowset = new List<List<string>>() { responseSample.RowSet[0] };
            var mappedObjects = SnowflakeDataMapper.MapTo<CustomClass>(responseSample.RowType, firstObjectRowset);
            var mappedObject = mappedObjects.FirstOrDefault();

            Assert.IsNotNull(mappedObject);
            Assert.AreEqual("Sometext", mappedObject.StringProperty);
            Assert.AreEqual(true, mappedObject.BoolProperty);
            Assert.AreEqual(7, mappedObject.IntProperty);
            Assert.AreEqual(27.6F, mappedObject.FloatProperty);
            Assert.AreEqual(19.239834M, mappedObject.DecimalProperty);
            Assert.AreEqual(Guid.Parse("e7412bbf-88ee-4149-b341-101e0f72ec7c"), mappedObject.GuidProperty);
            Assert.AreEqual(new byte[] { 0, 128, 255 }, mappedObject.ByteArrayProperty);

            var dateTime = DateTime.ParseExact("2020-09-13 12:26:40.0000000", "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
            Assert.AreEqual(dateTime, mappedObject.DateTimeProperty);

            var dateTimeOffset = DateTimeOffset.ParseExact("2020-09-13 12:26:40.0000000", "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            Assert.AreEqual(dateTimeOffset, mappedObject.DateTimeOffsetProperty);
        }

        [Test]
        public void ResponseWithValues_MapTo_CustomClassWithNullableProps()
        {
            var responseSample = GetFakeResponse();
            var firstObjectRowset = new List<List<string>>() { responseSample.RowSet[0] };
            var mappedObjects = SnowflakeDataMapper.MapTo<CustomClassNullables>(responseSample.RowType, firstObjectRowset);
            var mappedObject = mappedObjects.FirstOrDefault();

            Assert.IsNotNull(mappedObject);
            Assert.AreEqual(true, mappedObject.BoolProperty);
            Assert.AreEqual(7, mappedObject.IntProperty);
            Assert.AreEqual(27.6F, mappedObject.FloatProperty);
            Assert.AreEqual(19.239834M, mappedObject.DecimalProperty);
            Assert.AreEqual(Guid.Parse("e7412bbf-88ee-4149-b341-101e0f72ec7c"), mappedObject.GuidProperty);

            var dateTime = DateTime.ParseExact("2020-09-13 12:26:40.0000000", "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
            Assert.AreEqual(dateTime, mappedObject.DateTimeProperty);

            var dateTimeOffset = DateTimeOffset.ParseExact("2020-09-13 12:26:40.0000000", "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            Assert.AreEqual(dateTimeOffset, mappedObject.DateTimeOffsetProperty);
        }

        [Test]
        public void ResponseWithStringNull_MapTo_CustomClassWithNullables()
        {
            var responseSample = GetFakeResponse();

            var firstObjectRowset = new List<List<string>>() { responseSample.RowSet[1] };
            var mappedObjects = SnowflakeDataMapper.MapTo<CustomClassNullables>(responseSample.RowType, firstObjectRowset);
            var mappedObject = mappedObjects.FirstOrDefault();

            Assert.IsNotNull(mappedObject);
            Assert.AreEqual(null, mappedObject.StringProperty);
            Assert.AreEqual(null, mappedObject.BoolProperty);
            Assert.AreEqual(null, mappedObject.IntProperty);
            Assert.AreEqual(null, mappedObject.FloatProperty);
            Assert.AreEqual(null, mappedObject.DecimalProperty);
            Assert.AreEqual(null, mappedObject.GuidProperty);
            Assert.AreEqual(null, mappedObject.DateTimeProperty);
            Assert.AreEqual(null, mappedObject.DateTimeOffsetProperty);
            Assert.AreEqual(null, mappedObject.ByteArrayProperty);
        }

        [Test]
        public void ResponseWithNull_MapTo_CustomClassWithNullables()
        {
            var responseSample = GetFakeResponse();

            var firstObjectRowset = new List<List<string>>() { responseSample.RowSet[2] };
            var mappedObjects = SnowflakeDataMapper.MapTo<CustomClassNullables>(responseSample.RowType, firstObjectRowset);
            var mappedObject = mappedObjects.FirstOrDefault();

            Assert.IsNotNull(mappedObject);
            Assert.AreEqual(null, mappedObject.StringProperty);
            Assert.AreEqual(null, mappedObject.BoolProperty);
            Assert.AreEqual(null, mappedObject.IntProperty);
            Assert.AreEqual(null, mappedObject.FloatProperty);
            Assert.AreEqual(null, mappedObject.DecimalProperty);
            Assert.AreEqual(null, mappedObject.GuidProperty);
            Assert.AreEqual(null, mappedObject.DateTimeProperty);
            Assert.AreEqual(null, mappedObject.DateTimeOffsetProperty);
            Assert.AreEqual(null, mappedObject.ByteArrayProperty);
        }

        [Test]
        public void ResponseWithValues_MapTo_SingleValue()
        {
            var responseSample = GetFakeResponse();
            var rowSet = responseSample.RowSet[0];
            var rowType = responseSample.RowType;

            var stringValue = SnowflakeDataMapper.MapTo<string>(rowType[0], rowSet[0]);
            Assert.AreEqual("Sometext", stringValue);

            var boolValue = SnowflakeDataMapper.MapTo<bool>(rowType[1], rowSet[1]);
            Assert.AreEqual(true, boolValue);

            var intValue = SnowflakeDataMapper.MapTo<int>(rowType[2], rowSet[2]);
            Assert.AreEqual(7, intValue);

            var floatValue = SnowflakeDataMapper.MapTo<float>(rowType[3], rowSet[3]);
            Assert.AreEqual(27.6F, floatValue);

            var decimalValue = SnowflakeDataMapper.MapTo<decimal>(rowType[4], rowSet[4]);
            Assert.AreEqual(19.239834M, decimalValue);

            var dateTimeExpected = DateTime.ParseExact("2020-09-13 12:26:40.0000000", "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
            var dateTimeValue = SnowflakeDataMapper.MapTo<DateTime>(rowType[5], rowSet[5]);
            Assert.AreEqual(dateTimeExpected, dateTimeValue);

            var dateTimeOffsetExpected = DateTimeOffset.ParseExact("2020-09-13 12:26:40.0000000", "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            var dateTimeOffsetValue = SnowflakeDataMapper.MapTo<DateTimeOffset>(rowType[6], rowSet[6]);
            Assert.AreEqual(dateTimeOffsetExpected, dateTimeOffsetValue);

            var guidValue = SnowflakeDataMapper.MapTo<Guid>(rowType[7], rowSet[7]);
            Assert.AreEqual(Guid.Parse("e7412bbf-88ee-4149-b341-101e0f72ec7c"), guidValue);

            var bytesValues = SnowflakeDataMapper.MapTo<byte[]>(rowType[8], rowSet[8]);
            Assert.AreEqual(new byte[] { 0, 128, 255 }, bytesValues);
        }

        [Test]
        public void ResponseWithStringNull_MapTo_SingleValueNullable()
        {
            var responseSample = GetFakeResponse();
            var rowSet = responseSample.RowSet[1];
            var rowType = responseSample.RowType;

            var boolValue = SnowflakeDataMapper.MapTo<bool?>(rowType[1], rowSet[1]);
            Assert.AreEqual(null, boolValue);

            var intValue = SnowflakeDataMapper.MapTo<int?>(rowType[2], rowSet[2]);
            Assert.AreEqual(null, intValue);

            var floatValue = SnowflakeDataMapper.MapTo<float?>(rowType[3], rowSet[3]);
            Assert.AreEqual(null, floatValue);

            var decimalValue = SnowflakeDataMapper.MapTo<decimal?>(rowType[4], rowSet[4]);
            Assert.AreEqual(null, decimalValue);

            var dateTimeValue = SnowflakeDataMapper.MapTo<DateTime?>(rowType[5], rowSet[5]);
            Assert.AreEqual(null, dateTimeValue);

            var dateTimeOffsetValue = SnowflakeDataMapper.MapTo<DateTimeOffset?>(rowType[6], rowSet[6]);
            Assert.AreEqual(null, dateTimeOffsetValue);

            var guidValue = SnowflakeDataMapper.MapTo<Guid?>(rowType[7], rowSet[7]);
            Assert.AreEqual(null, guidValue);
        }

        [Test]
        public void ResponseWithNull_MapTo_SingleValueNullable()
        {
            var responseSample = GetFakeResponse();
            var rowSet = responseSample.RowSet[2];
            var rowType = responseSample.RowType;

            var boolValue = SnowflakeDataMapper.MapTo<bool?>(rowType[1], rowSet[1]);
            Assert.AreEqual(null, boolValue);

            var intValue = SnowflakeDataMapper.MapTo<int?>(rowType[2], rowSet[2]);
            Assert.AreEqual(null, intValue);

            var floatValue = SnowflakeDataMapper.MapTo<float?>(rowType[3], rowSet[3]);
            Assert.AreEqual(null, floatValue);

            var decimalValue = SnowflakeDataMapper.MapTo<decimal?>(rowType[4], rowSet[4]);
            Assert.AreEqual(null, decimalValue);

            var dateTimeValue = SnowflakeDataMapper.MapTo<DateTime?>(rowType[5], rowSet[5]);
            Assert.AreEqual(null, dateTimeValue);

            var dateTimeOffsetValue = SnowflakeDataMapper.MapTo<DateTimeOffset?>(rowType[6], rowSet[6]);
            Assert.AreEqual(null, dateTimeOffsetValue);

            var guidValue = SnowflakeDataMapper.MapTo<Guid?>(rowType[7], rowSet[7]);
            Assert.AreEqual(null, guidValue);
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

            response.RowSet.Add(new List<string>()
            {
                "Sometext",
                "true",
                "7",
                "27.6",
                "19.239834",
                "1600000000.000000000",
                "1600000000.000000000",
                "e7412bbf-88ee-4149-b341-101e0f72ec7c",
                "0080ff"
            });

            response.RowSet.Add(new List<string>()
            {
                "null",
                "null",
                "null",
                "null",
                "null",
                "null",
                "null",
                "null",
                "null"
            });


            response.RowSet.Add(new List<string>()
            {
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            });

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
            public string StringProperty { get; set; }

            public bool? BoolProperty { get; set; }

            public int? IntProperty { get; set; }

            public float? FloatProperty { get; set; }

            public decimal? DecimalProperty { get; set; }

            public DateTime? DateTimeProperty { get; set; }

            public DateTimeOffset? DateTimeOffsetProperty { get; set; }

            public Guid? GuidProperty { get; set; }

            public byte[] ByteArrayProperty { get; set; }
        }
    }
}