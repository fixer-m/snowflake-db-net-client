namespace Snowflake.Client.Json
{
    public class ColumnDescription
    {
        public string Name { get; set; }
        public long? ByteLength { get; set; }
        public long? Length { get; set; }
        public string Type { get; set; }
        public long? Scale { get; set; }
        public long? Precision { get; set; }
        public bool Nullable { get; set; }
    }
}