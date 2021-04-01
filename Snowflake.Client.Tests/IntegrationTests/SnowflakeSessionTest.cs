using NUnit.Framework;
using Snowflake.Client.Model;
using Snowflake.Client.Tests.IntegrationTests.Models;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Snowflake.Client.Tests.IntegrationTests
{
    [TestFixture]
    public class SnowflakeSessionTest
    {
        private readonly SnowflakeConnectionInfo _conectionInfo;

        public SnowflakeSessionTest()
        {
            var configJson = File.ReadAllText("testconfig.json");
            var testParameters = JsonSerializer.Deserialize<TestConfiguration>(configJson, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            _conectionInfo = testParameters.Connection;
        }

        [Test]
        public async Task InitNewSession()
        {
            var snowflakeClient = new SnowflakeClient(_conectionInfo.User, _conectionInfo.Password, _conectionInfo.Account, _conectionInfo.Region);

            var sessionInitialized = await snowflakeClient.InitNewSessionAsync();

            Assert.IsTrue(sessionInitialized);
            Assert.IsNotNull(snowflakeClient.SnowflakeSession);
        }

        [Test]
        public async Task RenewSession()
        {
            var snowflakeClient = new SnowflakeClient(_conectionInfo.User, _conectionInfo.Password, _conectionInfo.Account, _conectionInfo.Region);

            var sessionInitialized = await snowflakeClient.InitNewSessionAsync();
            var firstSessionToken = snowflakeClient.SnowflakeSession.SessionToken;

            var sessionRenewed = await snowflakeClient.RenewSessionAsync();
            var secondSessionToken = snowflakeClient.SnowflakeSession.SessionToken;

            Assert.IsTrue(sessionInitialized);
            Assert.IsTrue(sessionRenewed);
            Assert.IsTrue(firstSessionToken != secondSessionToken);
        }

        [Test]
        public async Task CloseSession()
        {
            var snowflakeClient = new SnowflakeClient(_conectionInfo.User, _conectionInfo.Password, _conectionInfo.Account, _conectionInfo.Region);

            var sessionInitialized = await snowflakeClient.InitNewSessionAsync();
            var sessionClosed = await snowflakeClient.CloseSessionAsync();

            Assert.IsTrue(sessionInitialized);
            Assert.IsTrue(sessionClosed);
            Assert.IsNull(snowflakeClient.SnowflakeSession);
        }
    }
}
