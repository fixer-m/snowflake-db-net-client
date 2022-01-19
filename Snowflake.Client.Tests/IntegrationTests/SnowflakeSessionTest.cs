using NUnit.Framework;
using System.Threading.Tasks;

namespace Snowflake.Client.Tests.IntegrationTests
{
    [TestFixture]
    public class SnowflakeSessionTest : IntegrationTestBase
    {
        [Test]
        public async Task InitNewSession()
        {
            var sessionInitialized = await _snowflakeClient.InitNewSessionAsync();

            Assert.IsTrue(sessionInitialized);
            Assert.IsNotNull(_snowflakeClient.SnowflakeSession);
        }

        [Test]
        public async Task RenewSession()
        {
            var sessionInitialized = await _snowflakeClient.InitNewSessionAsync();
            var firstSessionToken = _snowflakeClient.SnowflakeSession.SessionToken;

            var sessionRenewed = await _snowflakeClient.RenewSessionAsync();
            var secondSessionToken = _snowflakeClient.SnowflakeSession.SessionToken;

            Assert.IsTrue(sessionInitialized);
            Assert.IsTrue(sessionRenewed);
            Assert.IsTrue(firstSessionToken != secondSessionToken);
        }

        [Test]
        public async Task CloseSession()
        {
            var sessionInitialized = await _snowflakeClient.InitNewSessionAsync();
            var sessionClosed = await _snowflakeClient.CloseSessionAsync();

            Assert.IsTrue(sessionInitialized);
            Assert.IsTrue(sessionClosed);
            Assert.IsNull(_snowflakeClient.SnowflakeSession);
        }
    }
}
