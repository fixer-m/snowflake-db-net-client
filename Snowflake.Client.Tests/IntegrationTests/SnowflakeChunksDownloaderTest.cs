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
        public async Task DownloadAndParseChunks_MultipleThreads()
        {
            // Will produce 6 chunks
            var query = "select top 40000 * from SNOWFLAKE_SAMPLE_DATA.TPCH_SF1000.SUPPLIER;";
            var result = await _snowflakeClient.QueryRawResponseAsync(query);

            var chunksDownloadInfo = new ChunksDownloadInfo() { ChunkHeaders = result.ChunkHeaders, Chunks = result.Chunks, Qrmk = result.Qrmk };
            var parsed = await ChunksDownloader.DownloadAndParseChunksAsync(chunksDownloadInfo);
            var totalRowCountInChunks = result.Chunks.Sum(c => c.RowCount);

            Assert.AreEqual(totalRowCountInChunks, parsed.Count);
        }

        [Test]
        public async Task DownloadAndParseChunks_SingleThread()
        {
            // Will produce 6 chunks
            var query = "select top 40000 * from SNOWFLAKE_SAMPLE_DATA.TPCH_SF1000.SUPPLIER;";
            var result = await _snowflakeClient.QueryRawResponseAsync(query);

            var chunksDownloadInfo = new ChunksDownloadInfo() { ChunkHeaders = result.ChunkHeaders, Chunks = result.Chunks, Qrmk = result.Qrmk };
            var parsed = await ChunksDownloader.DownloadAndParseChunksSingleThreadAsync(chunksDownloadInfo);
            var totalRowCountInChunks = result.Chunks.Sum(c => c.RowCount);

            Assert.AreEqual(totalRowCountInChunks, parsed.Count);
        }

        [Test]
        public async Task QueryAndMap_ResponseWithChunks()
        {
            var selectCount = 10000;
            var query = $"select top {selectCount} * from SNOWFLAKE_SAMPLE_DATA.TPCH_SF1000.SUPPLIER;";
            var result = await _snowflakeClient.QueryAsync<Supplier>(query);
            var records = result.ToList();

            Assert.AreEqual(selectCount, records.Count);
        }

        [Test]
        public async Task QueryAndMap_ResponseWithChunksAndRowset()
        {
            var selectCount = 1370;
            var query = $"select top {selectCount} * from SNOWFLAKE_SAMPLE_DATA.TPCH_SF1000.SUPPLIER;";
            var result = await _snowflakeClient.QueryAsync<Supplier>(query);
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