using NUnit.Framework;
using Snowflake.Client.Model;
using System.Linq;
using System.Threading.Tasks;

namespace Snowflake.Client.Tests.IntegrationTests
{
    [TestFixture]
    public class SnowflakeChunksDownloaderTest : IntegrationTestBase
    {
        [Test]
        public async Task DownloadAndParseChunks()
        {
            var result = await _snowflakeClient.QueryRawResponseAsync("select top 10000 * from SNOWFLAKE_SAMPLE_DATA.TPCH_SF1000.SUPPLIER;");

            var chunksDownloadInfo = new ChunksDownloadInfo() { ChunkHeaders = result.ChunkHeaders, Chunks = result.Chunks, Qrmk = result.Qrmk };
            var parsed = await ChunksDownloader.DownloadAndParseChunksAsync(chunksDownloadInfo);

            var totalRowCountInChunks = result.Chunks.Sum(c => c.RowCount);
            Assert.AreEqual(totalRowCountInChunks, parsed.Count);
        }

        [Test]
        public async Task QueryAndMap_ResponseWithChunks()
        {
            var selectCount = 10000;
            var result = await _snowflakeClient.QueryAsync<Supplier>($"select top {selectCount} * from SNOWFLAKE_SAMPLE_DATA.TPCH_SF1000.SUPPLIER;");
            var records = result.ToList();

            Assert.AreEqual(selectCount, records.Count);
        }
    }

    internal class Supplier
    {
        public long? S_Suppkey { get; set; }
        public string S_Name { get; set; }
        public string S_Address { get; set; }
        public int? S_Nationkey { get; set; }
        public string S_Phone { get; set; }
        public float? S_Acctbal { get; set; }
        public string S_Comment { get; set; }
    }
}