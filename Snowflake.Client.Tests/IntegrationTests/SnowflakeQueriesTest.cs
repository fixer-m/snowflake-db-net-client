using NUnit.Framework;
using System.Threading.Tasks;

namespace Snowflake.Client.Tests.IntegrationTests
{
    [TestFixture]
    public class SnowflakeQueriesTest : IntegrationTestBase
    {
        [Test]
        public async Task ExecuteScalar_String()
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
        public async Task ExecuteScalar_Number()
        {
            var result = await _snowflakeClient.ExecuteScalarAsync("SELECT 1;");
            Assert.AreEqual("1", result);
        }
        
        [Test]
        public async Task ExecuteScalar_Typed_String()
        {
            var result = await _snowflakeClient.ExecuteScalarAsync<string>("SELECT CURRENT_USER();");
            Assert.IsTrue(!string.IsNullOrWhiteSpace(result));
        }
        
        [Test]
        public async Task ExecuteScalar_Typed_Null()
        {
            var result = await _snowflakeClient.ExecuteScalarAsync<string>("SELECT 1 WHERE 2 > 3;");
            Assert.IsNull(result);
        }

        [Test]
        public async Task ExecuteScalar_Typed_Number()
        {
            var result = await _snowflakeClient.ExecuteScalarAsync<int>("SELECT 1;");
            Assert.AreEqual(1, result);
        }
        
        [Test]
        public async Task Execute()
        {
            // todo: do temporary insert to get affected rows > 0

            var affectedRows = await _snowflakeClient.ExecuteAsync("SELECT 1;");
            Assert.AreEqual(-1, affectedRows);
        }

        [Test]
        public async Task QueryRawResponse()
        {
            var result = await _snowflakeClient.QueryRawResponseAsync("SELECT CURRENT_USER();");
            Assert.IsNotNull(result);
        }
    }
}
