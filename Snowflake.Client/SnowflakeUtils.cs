using System;
using System.Linq;
using Snowflake.Client.Json;
using Snowflake.Client.Model;

namespace Snowflake.Client
{
    // Based on https://github.com/snowflakedb/snowflake-connector-net/blob/master/Snowflake.Data/Core/ResultSetUtil.cs
    public class SnowflakeUtils
    {
        public static long GetAffectedRowsCount(QueryExecResponse response)
        {
            int statementTypeId = response.Data.StatementTypeId;

            if (!Enum.IsDefined(typeof(SnowflakeStatementType), statementTypeId))
                return 0;

            long updateCount = 0;
            var statementType = (SnowflakeStatementType)statementTypeId;
            switch (statementType)
            {
                case SnowflakeStatementType.INSERT:
                case SnowflakeStatementType.UPDATE:
                case SnowflakeStatementType.DELETE:
                case SnowflakeStatementType.MERGE:
                case SnowflakeStatementType.MULTI_INSERT:
                    updateCount = response.Data.RowSet[0].Sum(cell => long.Parse(cell));
                    break;

                case SnowflakeStatementType.COPY:
                    var rowsLoadedColumn = response.Data.RowType.FirstOrDefault(c => c.Name == "rows_loaded");
                    if (rowsLoadedColumn != null)
                    {
                        var rowsLoadedColumnIndex = response.Data.RowType.IndexOf(rowsLoadedColumn);
                        updateCount = long.Parse(response.Data.RowSet[0][rowsLoadedColumnIndex]);
                    }
                    break;

                case SnowflakeStatementType.SELECT:
                    updateCount = -1;
                    break;

                default:
                    updateCount = 0;
                    break;
            }

            return updateCount;
        }
    }
}