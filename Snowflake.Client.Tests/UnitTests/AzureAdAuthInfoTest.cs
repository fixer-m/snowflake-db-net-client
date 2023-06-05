using System;
using NUnit.Framework;
using Snowflake.Client.Tests.Models;
using Snowflake.Client.Model;
using System.IO;
using System.Text.Json;

namespace Snowflake.Client.Tests.IntegrationTests
{
    [TestFixture]
    public class AzureAdAuthInfoTests
    {
        protected readonly AzureAdAuthInfo _azureAdAuthInfo;

        public AzureAdAuthInfoTests()
        {
            var configJson = File.ReadAllText("testconfig.json");
            var testParameters = JsonSerializer.Deserialize<TestConfiguration>(configJson, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            var connectionInfo = testParameters.Connection;

            _azureAdAuthInfo = new AzureAdAuthInfo(
                testParameters.AdClientId,
                testParameters.AdClientSecret,
                testParameters.AdServicePrincipalObjectId,
                testParameters.AdTenantId,
                testParameters.AdScope,
                connectionInfo.Region,
                connectionInfo.Account,
                connectionInfo.User,
                connectionInfo.Host,
                connectionInfo.Role);
        }
    }
}
