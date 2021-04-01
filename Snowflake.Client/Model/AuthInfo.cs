using System;
using System.Linq;

namespace Snowflake.Client.Model
{
    /// <summary>
    /// Snowflake Authentication information.
    /// </summary>
    public class AuthInfo
    {
        /// <summary>
        /// Your Snofwlake account name
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
        /// Used to build Snowflake hostname: {Account}.{Region}.{Cloud}.snowflakecomputing.com
        /// Required for all, except for US West Oregon (us-west-2).
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Cloud tag. Specify only if your Snowflake URL has it.
        /// Allowed values: "aws", "azure", "gcp". 
        /// Used to build Snowflake hostname: {Account}.{Region}.{Cloud}.snowflakecomputing.com
        /// </summary>
        public string Cloud { get; set; }

        public AuthInfo()
        {
        }

        public AuthInfo(string user, string password, string account, string region = null, string cloud = null) : this()
        {
            User = user;
            Password = password;
            Account = account;

            if (!string.IsNullOrEmpty(region))
                Region = region;

            if (!string.IsNullOrEmpty(region))
                Cloud = cloud;
        }

        public string BuildHostName()
        {
            Validate();

            var hostname = $"{ReplaceUnderscores(Account)}.";

            if (!string.IsNullOrEmpty(Region) && Region.ToLower() != "us-west-2")
                hostname += $"{Region}.";

            if (!string.IsNullOrEmpty(Cloud))
                hostname += $"{Cloud}.";

            hostname += "snowflakecomputing.com";

            return hostname.ToLower();
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(Account))
                throw new Exception("Account name cannot be empty.");

            if (string.IsNullOrEmpty(Region) && !string.IsNullOrEmpty(Cloud))
                throw new Exception("Region is empty, but Cloud is specified.");

            var allowedClouds = new string[] {"aws", "azure", "gcp"};

            if (!string.IsNullOrEmpty(Cloud) && !allowedClouds.Contains(Cloud))
                throw new Exception($"Cloud tag '{Cloud}' is not allowed. Available values: 'aws', 'azure', 'gcp'.");
        }

        // Undescores in hostname will lead to SSL cert verification issue.
        // See https://github.com/snowflakedb/snowflake-connector-net/issues/160#issuecomment-692883663
        private string ReplaceUnderscores(string account)
        {
            return account.Replace("_", "-");
        }

        public override string ToString()
        {
            return $"Account: {Account}; User: {User}; Region: {Region}; Cloud: {Cloud}";
        }
    }
}