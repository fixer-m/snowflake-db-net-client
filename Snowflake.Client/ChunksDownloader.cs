using Snowflake.Client.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
        private const string SSE_C_ALGORITHM = "x-amz-server-side-encryption-customer-algorithm";
        private const string SSE_C_KEY = "x-amz-server-side-encryption-customer-key";
        private const string SSE_C_AES = "AES256";

        private static readonly HttpClient _httpClient;

        static ChunksDownloader()
        {
            var httpClientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };

            _httpClient = new HttpClient(httpClientHandler)
            {
                Timeout = TimeSpan.FromHours(1)
            };
        }

        public static async Task<List<List<string>>> DownloadAndParseChunksAsync(ChunksDownloadInfo chunksDownloadInfo, CancellationToken ct = default)
        {
            var rowset = new List<List<string>>();

            foreach (var chunk in chunksDownloadInfo.Chunks)
            {
                var downloadRequest = BuildChunkDownloadRequest(chunk, chunksDownloadInfo.ChunkHeaders, chunksDownloadInfo.Qrmk);
                var chunkRowSet = await GetChunkContentAsync(downloadRequest, ct);

                rowset.AddRange(chunkRowSet);
            }

            return rowset;
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
                request.Headers.Add(SSE_C_ALGORITHM, SSE_C_AES);
                request.Headers.Add(SSE_C_KEY, qrmk);
            }

            return request;
        }

        private static async ValueTask<List<List<string>>> GetChunkContentAsync(HttpRequestMessage request, CancellationToken ct = default)
        {
            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false))
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
        /// To parse it as a json (array of strings), we need to preappend '[' and append ']' to the stream 
        /// </summary>
        private static Stream BuildDeserializableStream(Stream content)
        {
            Stream openBracket = new MemoryStream(Encoding.UTF8.GetBytes("["));
            Stream closeBracket = new MemoryStream(Encoding.UTF8.GetBytes("]"));

            return new ConcatenatedStream(new Stream[3] { openBracket, content, closeBracket });
        }
    }
}
