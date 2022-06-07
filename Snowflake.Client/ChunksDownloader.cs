using Snowflake.Client.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Snowflake.Client
{
    public static class ChunksDownloader
    {
        private const string SF_SSE_C_ALGORITHM = "x-amz-server-side-encryption-customer-algorithm";
        private const string SF_SSE_C_KEY = "x-amz-server-side-encryption-customer-key";
        private const string SF_SSE_C_AES = "AES256";

        private static readonly HttpClient HttpClient;

        static ChunksDownloader()
        {
            var httpClientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };

            HttpClient = new HttpClient(httpClientHandler)
            {
                Timeout = TimeSpan.FromHours(1)
            };
        }

        public static async Task<List<List<string>>> DownloadAndParseChunksAsync(ChunksDownloadInfo chunksDownloadInfo, CancellationToken ct = default)
        {
            var rowSet = new List<List<string>>();

            foreach (var downloadRequest in chunksDownloadInfo.Chunks.Select(chunk => BuildChunkDownloadRequest(chunk, chunksDownloadInfo.ChunkHeaders, chunksDownloadInfo.Qrmk)))
            {
                var chunkRowSet = await GetChunkContentAsync(downloadRequest, ct);

                rowSet.AddRange(chunkRowSet);
            }

            return rowSet;
        }

        private static HttpRequestMessage BuildChunkDownloadRequest(ExecResponseChunk chunk, Dictionary<string, string> chunkHeaders, string qrmk)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(chunk.Url)
            };

            if (chunkHeaders != null)
            {
                foreach (var header in chunkHeaders)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            else
            {
                request.Headers.Add(SF_SSE_C_ALGORITHM, SF_SSE_C_AES);
                request.Headers.Add(SF_SSE_C_KEY, qrmk);
            }

            return request;
        }

        private static async ValueTask<List<List<string>>> GetChunkContentAsync(HttpRequestMessage request, CancellationToken ct = default)
        {
            using (var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    var concatStream = BuildDeserializableStream(stream);
                    return await JsonSerializer.DeserializeAsync<List<List<string>>>(concatStream, cancellationToken: ct);
                }
            }
        }

        /// <summary>
        /// Content from AWS S3 in format of 
        ///     ["val1", "val2", null, ...],
        ///     ["val3", "val4", null, ...],
        ///     ...
        /// To parse it as a json (array of strings), we need to pre-append '[' and append ']' to the stream 
        /// </summary>
        private static Stream BuildDeserializableStream(Stream content)
        {
            Stream openBracket = new MemoryStream(Encoding.UTF8.GetBytes("["));
            Stream closeBracket = new MemoryStream(Encoding.UTF8.GetBytes("]"));

            return new ConcatenatedStream(new[] { openBracket, content, closeBracket });
        }
    }
}
