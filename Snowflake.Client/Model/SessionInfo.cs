namespace Snowflake.Client.Model
{
    /// <summary>
    /// Represents current or desired Snowflake session objects
    /// </summary>
    public class SessionInfo
    {
        public string Role { get; set; }
        public string Schema { get; set; }
        public string Database { get; set; }
        public string Warehouse { get; set; }

        public override string ToString()
        {
            return $"Role: {Role}; WH: {Schema}; DB: {Database}; Schema: {Schema}";
        }
    }
}