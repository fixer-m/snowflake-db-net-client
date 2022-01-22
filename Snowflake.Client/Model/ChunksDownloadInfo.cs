using Snowflake.Client.Json;
using System.Collections.Generic;

namespace Snowflake.Client
{
    public class ChunksDownloadInfo
    {
        public List<ExecResponseChunk> Chunks { get; set; }
        public string Qrmk { get; set; }
        public Dictionary<string, string> ChunkHeaders { get; set; }
    }
}
