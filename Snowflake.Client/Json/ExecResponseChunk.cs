namespace Snowflake.Client.Json
{
    public class ExecResponseChunk
    {
        public string Url { get; set; }
        public int RowCount { get; set; }
        public long UncompressedSize { get; set; }
    }
}