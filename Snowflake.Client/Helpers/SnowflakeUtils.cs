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

        // Based on: https://docs.snowflake.com/en/user-guide/admin-account-identifier.html#locator-formats-by-cloud-platform-and-region
        internal static string GetCloudTagByRegion(string region)
        {
            if (string.IsNullOrEmpty(region))
                return "";

            // User can pass "us-east-2.aws"
            if (region.Contains("."))
                return "";

            var regionTags = new Dictionary<string, string>();

            regionTags.Add("us-west-2", ""); // "default" 

            regionTags.Add("us-east-2", "aws");
            regionTags.Add("us-east-1", "");
            regionTags.Add("us-east-1-gov", "aws");
            regionTags.Add("ca-central-1", "aws");
            regionTags.Add("eu-west-1", "");
            regionTags.Add("eu-west-2", "aws");
            regionTags.Add("eu-central-1", "");
            regionTags.Add("ap-northeast-1", "aws");
            regionTags.Add("ap-south-1", "aws");
            regionTags.Add("ap-southeast-1", "");
            regionTags.Add("ap-southeast-2", "");
            regionTags.Add("us-central1", "gcp");
            regionTags.Add("europe-west2", "gcp");
            regionTags.Add("europe-west4", "gcp");
            regionTags.Add("west-us-2", "azure");
            regionTags.Add("east-us-2", "azure");
            regionTags.Add("us-gov-virginia", "azure");
            regionTags.Add("canada-central", "azure");
            regionTags.Add("west-europe", "azure");
            regionTags.Add("switzerland-north", "azure");
            regionTags.Add("southeast-asia", "azure");
            regionTags.Add("australia-east", "azure");

            regionTags.TryGetValue(region, out string cloudTag);
            return cloudTag ?? "";
        }

    }
}