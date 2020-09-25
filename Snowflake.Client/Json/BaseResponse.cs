namespace Snowflake.Client.Json
{
    public abstract class BaseResponse
    {
        public string Message { get; set; }
        public string Code { get; set; }
        public bool Success { get; set; }
        public string Headers { get; set; }
    }
}