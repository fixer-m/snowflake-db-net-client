namespace Snowflake.Client.Model
{
    public static class SnowflakeConst
    {
        public const string SF_SESSION_PATH = "/session";

        public const string SF_LOGIN_PATH = SF_SESSION_PATH + "/v1/login-request";

        public const string SF_TOKEN_REQUEST_PATH = SF_SESSION_PATH + "/token-request";

        public const string SF_AUTHENTICATOR_REQUEST_PATH = SF_SESSION_PATH + "/authenticator-request";

        public const string SF_QUERY_PATH = "/queries/v1/query-request";

        public const string SF_QUERY_CANCEL_PATH = "/queries/v1/abort-request";

        public const string SF_QUERY_WAREHOUSE = "warehouse";

        public const string SF_QUERY_DB = "databaseName";

        public const string SF_QUERY_SCHEMA = "schemaName";

        public const string SF_QUERY_ROLE = "roleName";

        public const string SF_QUERY_REQUEST_ID = "requestId";

        public const string SF_QUERY_REQUEST_GUID = "request_guid";

        public const string SF_QUERY_START_TIME = "clientStartTime";

        public const string SF_QUERY_RETRY_COUNT = "retryCount";

        public const string SF_QUERY_SESSION_DELETE = "delete";
    }
}