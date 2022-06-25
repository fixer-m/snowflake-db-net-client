using System.Collections.Generic;
using Snowflake.Client.Json;

namespace Snowflake.Client.Model
{
    public class ChunksDownloadInfo
    {
        public List<ExecResponseChunk> Chunks { get; set; }
        public string Qrmk { get; set; }
        public Dictionary<string, string> ChunkHeaders { get; set; }
    }
}
