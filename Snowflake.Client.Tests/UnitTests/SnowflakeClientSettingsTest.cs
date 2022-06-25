using NUnit.Framework;
using Snowflake.Client.Model;
using System;

namespace Snowflake.Client.Tests.UnitTests
{
    [TestFixture]
    public class SnowflakeClientSettingsTest
    {
        [Test]
        [TestCase("user", "pw", "account")]
        public void AuthInfo_Ctor(string user, string password, string account)
        {
            var authInfo = new AuthInfo(user, password, account);
            var settings = new SnowflakeClientSettings(authInfo);

            Assert.AreEqual($"{account}.snowflakecomputing.com", settings.UrlInfo.Host);
            Assert.AreEqual("https", settings.UrlInfo.Protocol);
            Assert.AreEqual(443, settings.UrlInfo.Port);

            Assert.AreEqual(user, settings.AuthInfo.User);
            Assert.AreEqual(password, settings.AuthInfo.Password);
            Assert.AreEqual(account, settings.AuthInfo.Account);
        }

        [Test]
        [TestCase("user", "pw", "account")]
        public void AuthInfo_Props(string user, string password, string account)
        {
            var authInfo = new AuthInfo() { User = user, Password = password, Account = account };
            var settings = new SnowflakeClientSettings(authInfo);

            Assert.AreEqual($"{account}.snowflakecomputing.com", settings.UrlInfo.Host);
            Assert.AreEqual("https", settings.UrlInfo.Protocol);
            Assert.AreEqual(443, settings.UrlInfo.Port);

            Assert.AreEqual(user, settings.AuthInfo.User);
            Assert.AreEqual(password, settings.AuthInfo.Password);
            Assert.AreEqual(account, settings.AuthInfo.Account);
        }

        [Test]
        public void AuthInfo_Ctor_AccountWithUnderscore()
        {
            var authInfo = new AuthInfo("user", "pw", "account_with_underscore");
            var settings = new SnowflakeClientSettings(authInfo);

            Assert.AreEqual("account-with-underscore.snowflakecomputing.com", settings.UrlInfo.Host);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void AuthInfo_Ctor_EmptyAccount(string account)
        {
            var authInfo = new AuthInfo("user", "pw", account);

            Assert.Throws<ArgumentException>(() =>
            {
                var snowflakeClientSettings = new SnowflakeClientSettings(authInfo);
            });
        }

        [Test]
        [TestCase("west-us-2", "azure")]
        [TestCase("east-us-2", "azure")]
        [TestCase("us-gov-virginia", "azure")]
        [TestCase("canada-central", "azure")]
        [TestCase("west-europe", "azure")]
        [TestCase("switzerland-north", "azure")]
        [TestCase("southeast-asia", "azure")]
        [TestCase("australia-east", "azure")]
        [TestCase("us-east-2", "aws")]
        [TestCase("us-east-1-gov", "aws")]
        [TestCase("ca-central-1", "aws")]
        [TestCase("eu-west-2", "aws")]
        [TestCase("ap-northeast-1", "aws")]
        [TestCase("ap-south-1", "aws")]
        [TestCase("us-central1", "gcp")]
        [TestCase("europe-west2", "gcp")]
        [TestCase("europe-west4", "gcp")]
        public void AuthInfo_Ctor_Regions(string region, string expectedCloud)
        {
            var authInfo = new AuthInfo("user", "pw", "account", region);
            var settings = new SnowflakeClientSettings(authInfo);

            Assert.AreEqual($"account.{region}.{expectedCloud}.snowflakecomputing.com", settings.UrlInfo.Host);
        }

        [Test]
        [TestCase("us-east-1")]
        [TestCase("eu-west-1")]
        [TestCase("eu-central-1")]
        [TestCase("ap-southeast-1")]
        [TestCase("ap-southeast-2")]
        public void AuthInfo_Ctor_Unique_Regions(string region)
        {
            var authInfo = new AuthInfo("user", "pw", "account", region);
            var settings = new SnowflakeClientSettings(authInfo);

            Assert.AreEqual($"account.{region}.snowflakecomputing.com", settings.UrlInfo.Host);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("us-west-2")]
        public void AuthInfo_Ctor_Default_Region(string region)
        {
            var authInfo = new AuthInfo("user", "pw", "account", region);
            var settings = new SnowflakeClientSettings(authInfo);

            Assert.AreEqual($"account.snowflakecomputing.com", settings.UrlInfo.Host);
        }

        [Test]
        [TestCase("ca-central-1.aws")]
        [TestCase("europe-west4.gcp")]
        public void AuthInfo_Ctor_RegionWithCloud(string region)
        {
            var authInfo = new AuthInfo("user", "pw", "account", region);
            var settings = new SnowflakeClientSettings(authInfo);

            Assert.AreEqual($"account.{region}.snowflakecomputing.com", settings.UrlInfo.Host);
        }

        [Test]
        public void UrlInfo_Ctor()
        {
            var urlInfo = new UrlInfo("account-url.snowflakecomputing.com");
            var settings = new SnowflakeClientSettings(new AuthInfo("user", "pw", "account"), null, urlInfo);

            Assert.AreEqual("account-url.snowflakecomputing.com", settings.UrlInfo.Host);
            Assert.AreEqual("https", settings.UrlInfo.Protocol);
            Assert.AreEqual(443, settings.UrlInfo.Port);
        }

        [Test]
        public void UrlInfo_Props()
        {
            var urlInfo = new UrlInfo() { Host = "account-url.snowflakecomputing.com" };
            var settings = new SnowflakeClientSettings(new AuthInfo("user", "pw", "account"), null, urlInfo);

            Assert.AreEqual("account-url.snowflakecomputing.com", settings.UrlInfo.Host);
            Assert.AreEqual("https", settings.UrlInfo.Protocol);
            Assert.AreEqual(443, settings.UrlInfo.Port);
        }

        [Test]
        public void UrlInfo_Explicit_Host()
        {
            var authInfo = new AuthInfo("user", "pw", "account-auth");
            var urlInfo = new UrlInfo("account-url.snowflakecomputing.com");
            var settings = new SnowflakeClientSettings(authInfo, null, urlInfo);

            Assert.AreEqual("account-url.snowflakecomputing.com", settings.UrlInfo.Host);
        }

        [Test]
        public void UrlInfo_Explicit_Host_Account_WithUnderscore()
        {
            var authInfo = new AuthInfo("user", "pw", "account");
            var urlInfo = new UrlInfo("account_with_underscore.snowflakecomputing.com");
            var settings = new SnowflakeClientSettings(authInfo, null, urlInfo);

            Assert.AreEqual($"account-with-underscore.snowflakecomputing.com", settings.UrlInfo.Host);
        }

        [Test]
        [TestCase("https://account-1.snowflakecomputing.com", "https", 443)]
        [TestCase("http://account-2.snowflakecomputing.com", "http", 80)]
        public void UrlInfo_Uri(string url, string expectedProtocol, int expectedPort)
        {
            var authInfo = new AuthInfo("user", "pw", "account");
            var urlInfo = new UrlInfo(new Uri(url));
            var settings = new SnowflakeClientSettings(authInfo, null, urlInfo);

            Assert.AreEqual(url.Replace(expectedProtocol + "://", ""), settings.UrlInfo.Host);
            Assert.AreEqual(expectedPort, settings.UrlInfo.Port);
            Assert.AreEqual(expectedProtocol, settings.UrlInfo.Protocol);
        }
    }
}