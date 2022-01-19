using Snowflake.Client.Tests.IntegrationTests.Models;
using System.IO;
using System.Text.Json;

namespace Snowflake.Client.Tests.IntegrationTests
{
    public class IntegrationTestBase
    {
        protected readonly SnowflakeClient _snowflakeClient;

        public IntegrationTestBase()
        {
            var configJson = File.ReadAllText("testconfig.json");
            var testParameters = JsonSerializer.Deserialize<TestConfiguration>(configJson, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            var conectionInfo = testParameters.Connection;

            _snowflakeClient = new SnowflakeClient(conectionInfo.User, conectionInfo.Password, conectionInfo.Account, conectionInfo.Region);
        }
    }
}