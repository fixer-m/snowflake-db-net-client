using Snowflake.Client.Helpers;
using System.Text.Json.Serialization;

namespace Snowflake.Client.Json
{
    public abstract class BaseResponse
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public string Headers { get; set; }

        [JsonConverter(typeof(QuotedNumbersToIntConverter))]
        public int? Code { get; set; }
    }
}