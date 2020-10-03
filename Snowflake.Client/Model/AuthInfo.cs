namespace Snowflake.Client.Model
{
    /// <summary>
    /// Snowflake Authentication information.
    /// </summary>
    public class AuthInfo
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public string User { get; set; }
        public string Region { get; set; }

        public AuthInfo()
        {
            Region = "us-east-1";
        }

        public AuthInfo(string user, string password, string account, string region = null) : this()
        {
            User = user;
            Password = password;
            Account = account;

            if (!string.IsNullOrEmpty(region))
                Region = region;
        }

        public string GetHostName()
        {
            if (Account != null && Region != null)
                return $"{Account}.{Region}.snowflakecomputing.com";

            return null;
        }
    }
}