using NUnit.Framework;
using System.Threading.Tasks;

namespace Snowflake.Client.Tests.IntegrationTests
{
    [TestFixture]
    public class SnowflakeQueriesTest : IntegrationTestBase
    {
        [Test]
        public async Task ExecuteScalar_WithResult()
        {
            var result = await _snowflakeClient.ExecuteScalarAsync("SELECT CURRENT_USER();");

            Assert.IsTrue(!string.IsNullOrWhiteSpace(result));
        }

        [Test]
        public async Task ExecuteScalar_Null()
        {
            var result = await _snowflakeClient.ExecuteScalarAsync("SELECT 1 WHERE 2 > 3;");

            Assert.IsNull(result);
        }

        [Test]
        public async Task Execute()
        {
            // todo: do temporary insert to get affected rows > 0

            var result = await _snowflakeClient.ExecuteAsync("SELECT 1;");

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
