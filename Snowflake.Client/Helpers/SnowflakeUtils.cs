using System;
using System.Collections.Generic;
using System.Linq;
using Snowflake.Client.Json;
using Snowflake.Client.Model;

namespace Snowflake.Client.Helpers
{
    internal static class SnowflakeUtils
    {
        // Based on https://github.com/snowflakedb/snowflake-connector-net/blob/master/Snowflake.Data/Core/ResultSetUtil.cs
        internal static long GetAffectedRowsCount(QueryExecResponse response)
        {
            var statementTypeId = response.Data.StatementTypeId;

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

                case SnowflakeStatementType.COPY_UNLOAD:
                    var rowsUnloadedColumn = response.Data.RowType.FirstOrDefault(c => c.Name == "rows_unloaded");
                    if (rowsUnloadedColumn != null)
                    {
                        var rowsUnloadedColumnIndex = response.Data.RowType.IndexOf(rowsUnloadedColumn);
                        updateCount = long.Parse(response.Data.RowSet[0][rowsUnloadedColumnIndex]);
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

        // Based on: https://docs.snowflake.com/en/user-guide/admin-account-identifier.html#locator-formats-by-cloud-platform-and-region
        internal static string GetCloudTagByRegion(string region)
        {
            if (string.IsNullOrEmpty(region))
                return "";

            // User can pass "us-east-2.aws"
            if (region.Contains("."))
                return "";

            var regionTags = new Dictionary<string, string>
            {
                { "us-west-2", "" }, // "default" 
                { "us-east-2", "aws" },
                { "us-east-1", "" },
                { "us-east-1-gov", "aws" },
                { "ca-central-1", "aws" },
                { "eu-west-1", "" },
                { "eu-west-2", "aws" },
                { "eu-central-1", "" },
                { "ap-northeast-1", "aws" },
                { "ap-south-1", "aws" },
                { "ap-southeast-1", "" },
                { "ap-southeast-2", "" },
                { "us-central1", "gcp" },
                { "europe-west2", "gcp" },
                { "europe-west4", "gcp" },
                { "west-us-2", "azure" },
                { "east-us-2", "azure" },
                { "us-gov-virginia", "azure" },
                { "canada-central", "azure" },
                { "west-europe", "azure" },
                { "switzerland-north", "azure" },
                { "southeast-asia", "azure" },
                { "australia-east", "azure" }
            };

            regionTags.TryGetValue(region, out var cloudTag);
            return cloudTag ?? "";
        }

    }
}