using NUnit.Framework;
using Snowflake.Client.Tests.IntegrationTests.Models;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Snowflake.Client.Tests.IntegrationTests
{
    [TestFixture]
    public class SnowflakeQueriesTest
    {
        private readonly SnowflakeClient _snowflakeClient;

        public SnowflakeQueriesTest()
        {
            var configJson = File.ReadAllText("testconfig.json");
            var testParameters = JsonSerializer.Deserialize<TestConfiguration>(configJson, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            var conectionInfo = testParameters.Connection;

            _snowflakeClient = new SnowflakeClient(conectionInfo.User, conectionInfo.Password, conectionInfo.Account, conectionInfo.Region);
        }

        [Test]
        public async Task ExecuteScalar_WithResult()
        {
            string result = await _snowflakeClient.ExecuteScalarAsync("SELECT CURRENT_USER();");

            Assert.IsTrue(!string.IsNullOrWhiteSpace(result));
        }

        [Test]
        public async Task ExecuteScalar_Null()
        {
            string result = await _snowflakeClient.ExecuteScalarAsync("SELECT 1 WHERE 2 > 3;");

            Assert.IsNull(result);
        }

        [Test]
        public async Task Execute()
        {
            // todo: do temporary insert to get affected rows > 0

            long result = await _snowflakeClient.ExecuteAsync("SELECT 1;");

            Assert.IsTrue(result == -1);
        }

        [Test]
        public async Task QueryRawResponse()
        {
            var result = await _snowflakeClient.QueryRawResponseAsync("SELECT CURRENT_USER();");

            Assert.IsNotNull(result);
        }
    }
}
