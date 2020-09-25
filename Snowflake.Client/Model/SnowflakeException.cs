using System;

namespace Snowflake.Client
{
    [Serializable]
    public class SnowflakeException : Exception
    {
        public string ErrorCode { get; private set; }

        public SnowflakeException()
        {
        }

        public SnowflakeException(string message) : base(message)
        {
        }

        public SnowflakeException(string message, string code) : base(message)
        {
            ErrorCode = code;
        }

        public SnowflakeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}