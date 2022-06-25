namespace Snowflake.Client.Model
{
    /// <summary>
    /// Snowflake Authentication information.
    /// </summary>
    public class AuthInfo
    {
        /// <summary>
        /// Your Snowflake account name
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// Your Snowflake user password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Your Snowflake username
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Region: "us-east-1", etc. 
        /// Required for all, except for US West Oregon (us-west-2).
        /// Used to build Snowflake hostname: Account.Region.Cloud.snowflakecomputing.com.
        /// </summary>
        public string Region { get; set; }

        public AuthInfo()
        {
        }

        public AuthInfo(string user, string password, string account, string region = null)
        {
            User = user;
            Password = password;
            Account = account;
            Region = region;
        }

        public override string ToString()
        {
            return $"Account: {Account}; User: {User}; Region: {Region};";
        }
    }
}