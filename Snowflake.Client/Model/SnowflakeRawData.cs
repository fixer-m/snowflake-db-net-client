using Snowflake.Client.Json;
using System.Collections.Generic;

namespace Snowflake.Client.Model
{
    /// <summary>
    /// Raw data returned from Snowflake REST API.
    /// </summary>
    public class SnowflakeRawData
    {
        public List<ColumnDescription> Columns { get; set; }

        public List<List<string>> Rows { get; set; }
    }
}