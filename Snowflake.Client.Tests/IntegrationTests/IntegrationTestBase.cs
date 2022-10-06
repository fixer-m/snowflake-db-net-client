using System.IO;
using System.Text.Json;
using Snowflake.Client.Tests.Models;

namespace Snowflake.Client.Tests.IntegrationTests
{
    public class IntegrationTestBase
    {
        protected readonly SnowflakeClient _snowflakeClient;

        public IntegrationTestBase()
        {
            var configJson = File.ReadAllText("testconfig.json");
            var testParameters = JsonSerializer.Deserialize<TestConfiguration>(configJson, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            var connectionInfo = testParameters.Connection;

            _snowflakeClient = new SnowflakeClient(new Model.AuthInfo
            {
                User = connectionInfo.User, 
                Password = connectionInfo.Password, 
                Account = connectionInfo.Account, 
                Region = connectionInfo.Region
            },
            new Model.SessionInfo
            {
                Warehouse = connectionInfo.Warehouse
            });
        }
    }
}