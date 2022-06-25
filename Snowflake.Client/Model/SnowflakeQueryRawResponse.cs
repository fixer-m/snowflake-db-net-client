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
            Parameters = responseData.Parameters;
            Columns = responseData.RowType;
            Rows = responseData.RowSet;
            Total = responseData.Total;
            Returned = responseData.Returned;
            QueryId = responseData.QueryId;
            SqlState = responseData.SqlState;
            DatabaseProvider = responseData.DatabaseProvider;
            FinalDatabaseName = responseData.FinalDatabaseName;
            FinalRoleName = responseData.FinalRoleName;
            FinalSchemaName = responseData.FinalSchemaName;
            FinalWarehouseName = responseData.FinalWarehouseName;
            NumberOfBinds = responseData.NumberOfBinds;
            StatementTypeId = responseData.StatementTypeId;
            Version = responseData.Version;
            Chunks = responseData.Chunks;
            Qrmk = responseData.Qrmk;
            ChunkHeaders = responseData.ChunkHeaders;
            GetResultUrl = responseData.GetResultUrl;
            ProgressDesc = responseData.ProgressDesc;
            QueryAbortAfterSecs = responseData.QueryAbortAfterSecs;
        }
    }
}