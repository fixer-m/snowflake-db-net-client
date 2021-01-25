using Snowflake.Client.Json;
using System.Collections.Generic;

namespace Snowflake.Client.Model
{
    public class SnowflakeQueryRawResponse
    {
        public List<NameValueParameter> Parameters { get; private set; }
        public List<ColumnDescription> Columns { get; private set; }
        public List<List<string>> Rows { get; private set; }
        public long Total { get; private set; }
        public long Returned { get; private set; }
        public string QueryId { get; private set; }
        public string SqlState { get; private set; }
        public string DatabaseProvider { get; private set; }
        public string FinalDatabaseName { get; private set; }
        public string FinalSchemaName { get; private set; }
        public string FinalWarehouseName { get; private set; }
        public string FinalRoleName { get; private set; }
        public int NumberOfBinds { get; private set; }
        public int StatementTypeId { get; private set; }
        public int Version { get; private set; }
        public List<ExecResponseChunk> Chunks { get; private set; }
        public string Qrmk { get; private set; }
        public Dictionary<string, string> ChunkHeaders { get; private set; }
        public string GetResultUrl { get; private set; }
        public string ProgressDesc { get; private set; }
        public long QueryAbortAfterSecs { get; private set; }

        public SnowflakeQueryRawResponse(QueryExecResponseData responseData)
        {
            this.Parameters = responseData.Parameters;
            this.Columns = responseData.RowType;
            this.Rows = responseData.RowSet;
            this.Total = responseData.Total;
            this.Returned = responseData.Returned;
            this.QueryId = responseData.QueryId;
            this.SqlState = responseData.SqlState;
            this.DatabaseProvider = responseData.DatabaseProvider;
            this.FinalDatabaseName = responseData.FinalDatabaseName;
            this.FinalRoleName = responseData.FinalRoleName;
            this.FinalSchemaName = responseData.FinalSchemaName;
            this.FinalWarehouseName = responseData.FinalWarehouseName;
            this.NumberOfBinds = responseData.NumberOfBinds;
            this.StatementTypeId = responseData.StatementTypeId;
            this.Version = responseData.Version;
            this.Chunks = responseData.Chunks;
            this.Qrmk = responseData.Qrmk;
            this.ChunkHeaders = responseData.ChunkHeaders;
            this.GetResultUrl = responseData.GetResultUrl;
            this.ProgressDesc = responseData.ProgressDesc;
            this.QueryAbortAfterSecs = responseData.QueryAbortAfterSecs;
        }
    }
}
