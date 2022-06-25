using System;

namespace Snowflake.Client.Model
{
    [Serializable]
    public class SnowflakeException : Exception
    {
        public int? Code { get; private set; }

        public SnowflakeException()
        {
        }

        public SnowflakeException(string message) : base(message)
        {
        }

        public SnowflakeException(string message, int? code) : base(message)
        {
            Code = code;
        }

        public SnowflakeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}