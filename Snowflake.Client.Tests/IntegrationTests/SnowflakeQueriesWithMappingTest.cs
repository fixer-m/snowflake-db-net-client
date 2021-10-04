using NUnit.Framework;
using Snowflake.Client.Tests.IntegrationTests.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Snowflake.Client.Tests.IntegrationTests
{
    [TestFixture]
    public class SnowflakeQueriesWithMappingTest
    {
        private readonly SnowflakeClient _snowflakeClient;

        public SnowflakeQueriesWithMappingTest()
        {
            var configJson = File.ReadAllText("testconfig.json");
            var testParameters = JsonSerializer.Deserialize<TestConfiguration>(configJson, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            var conectionInfo = testParameters.Connection;

            _snowflakeClient = new SnowflakeClient(conectionInfo.User, conectionInfo.Password, conectionInfo.Account, conectionInfo.Region);
        }

        [Test]
        public async Task QueryAndMap_SimpleTypes()
        {
            await CreateAndPopulateTableWithSimpleDataTypes();

            var result = await _snowflakeClient.QueryAsync<SimpleDataTypes>("SELECT * FROM DEMO_DB.PUBLIC.DATATYPES_SIMPLE;");
            var records = result.ToList();

            ValidateSimpleRecords(records);

            await RemoveSimpleDatatypesTable();
        }

        private void ValidateSimpleRecords(List<SimpleDataTypes> records)
        {
            Assert.AreEqual(1, records[0].Id);
            Assert.AreEqual(1, records[0].SomeInt);
            Assert.AreEqual(2.5F, records[0].SomeFloat);
            Assert.AreEqual("some-text", records[0].SomeVarchar);
            Assert.AreEqual(true, records[0].SomeBoolean);
            Assert.AreEqual(new byte[] { 119, 111, 119 }, records[0].SomeBinary);

            Assert.AreEqual(2, records[1].Id);
            Assert.AreEqual(0, records[1].SomeInt);
            Assert.AreEqual(777.0F, records[1].SomeFloat);
            Assert.AreEqual("", records[1].SomeVarchar);
            Assert.AreEqual(false, records[1].SomeBoolean);
            Assert.AreEqual(null, records[1].SomeBinary);

            Assert.AreEqual(3, records[2].Id);
            Assert.AreEqual(-1, records[2].SomeInt);
            Assert.AreEqual(-2.5F, records[2].SomeFloat);
            Assert.AreEqual("some-text\r\n with rn", records[2].SomeVarchar);
            Assert.AreEqual(null, records[2].SomeBoolean);
            Assert.AreEqual(new byte[] { 55, 55, 54, 70, 55, 55 }, records[2].SomeBinary);

            Assert.AreEqual(4, records[3].Id);
            Assert.AreEqual(null, records[3].SomeInt);
            Assert.AreEqual(null, records[3].SomeFloat);
            Assert.AreEqual(null, records[3].SomeVarchar);
            Assert.AreEqual(null, records[3].SomeBoolean);
            Assert.AreEqual(null, records[3].SomeBinary);
        }

        private async Task CreateAndPopulateTableWithSimpleDataTypes()
        {
            var query = "CREATE OR REPLACE TABLE DEMO_DB.PUBLIC.DATATYPES_SIMPLE " +
                        "(ID INT, SomeInt INT, SomeFloat FLOAT, SomeVarchar VARCHAR, SomeBoolean BOOLEAN, SomeBinary BINARY);";

            var result = await _snowflakeClient.ExecuteScalarAsync(query);

            var insertQuery1 = "INSERT INTO DEMO_DB.PUBLIC.DATATYPES_SIMPLE (ID, SomeInt, SomeFloat, SomeVarchar, SomeBoolean, SomeBinary) " +
                               "SELECT 1, 1, 2.5, 'some-text', true, to_binary(hex_encode('wow'));";

            var insertQuery2 = "INSERT INTO DEMO_DB.PUBLIC.DATATYPES_SIMPLE (ID, SomeInt, SomeFloat, SomeVarchar, SomeBoolean, SomeBinary) " +
                               "SELECT 2, 0, 777.0, '', false, null;";

            var insertQuery3 = "INSERT INTO DEMO_DB.PUBLIC.DATATYPES_SIMPLE (ID, SomeInt, SomeFloat, SomeVarchar, SomeBoolean, SomeBinary) " +
                               "SELECT 3, -1, -2.5, 'some-text\r\n with rn', null, to_binary(hex_encode('wow'), 'UTF-8');";

            var insertQuery4 = "INSERT INTO DEMO_DB.PUBLIC.DATATYPES_SIMPLE (ID, SomeInt, SomeFloat, SomeVarchar, SomeBoolean, SomeBinary) " +
                               "SELECT 4, null, null, null, null, null;";

            var insertion1 = await _snowflakeClient.ExecuteAsync(insertQuery1);
            var insertion2 = await _snowflakeClient.ExecuteAsync(insertQuery2);
            var insertion3 = await _snowflakeClient.ExecuteAsync(insertQuery3);
            var insertion4 = await _snowflakeClient.ExecuteAsync(insertQuery4);
        }

        private async Task<string> RemoveSimpleDatatypesTable()
        {
            var query = "DROP TABLE IF EXISTS DEMO_DB.PUBLIC.DATATYPES_SIMPLE;";
            var result = await _snowflakeClient.ExecuteScalarAsync(query);

            return result;
        }
    }

    public class SimpleDataTypes
    {
        public int Id { get; set; }
        public int? SomeInt { get; set; }
        public float? SomeFloat { get; set; }
        public string SomeVarchar { get; set; }
        public bool? SomeBoolean { get; set; }
        public byte[] SomeBinary { get; set; }
    }
}
