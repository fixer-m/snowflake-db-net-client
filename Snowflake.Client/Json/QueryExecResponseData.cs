using System.Collections.Generic;

namespace Snowflake.Client.Json
{
    public class QueryExecResponseData
    {
        public List<NameValueParameter> Parameters { get; set; }
        public List<ColumnDescription> RowType { get; set; }
        public List<List<string>> RowSet { get; set; }
        public long Total { get; set; }
        public long Returned { get; set; }
        public string QueryId { get; set; }
        public string SqlState { get; set; }
        public string DatabaseProvider { get; set; }
        public string FinalDatabaseName { get; set; }
        public string FinalSchemaName { get; set; }
        public string FinalWarehouseName { get; set; }
        public string FinalRoleName { get; set; }
        public int NumberOfBinds { get; set; }
        public int StatementTypeId { get; set; }
        public int Version { get; set; }
        public List<ExecResponseChunk> Chunks { get; set; }
        public string Qrmk { get; set; }
        public Dictionary<string, string> ChunkHeaders { get; set; }
        public string GetResultUrl { get; set; }
        public string ProgressDesc { get; set; }
        public long QueryAbortAfterSecs { get; set; }
    }
}