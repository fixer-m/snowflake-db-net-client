using System.Text.Json.Serialization;

namespace Snowflake.Client.Json
{
    public class SessionInfoRaw
    {
        [JsonPropertyName("databaseName")]
        public string DatabaseName { get; set; }

        [JsonPropertyName("schemaName")]
        public string SchemaName { get; set; }

        [JsonPropertyName("warehouseName")]
        public string WarehouseName { get; set; }

        [JsonPropertyName("roleName")]
        public string RoleName { get; set; }
    }
}