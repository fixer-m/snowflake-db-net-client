using Microsoft.Identity.Client;
using Moq;
using NUnit.Framework;
using Snowflake.Client;
using Snowflake.Client.Model;
using Snowflake.Client.Tests.IntegrationTests;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Snowflake.Client.Tests
{
    public class AzureAdTokenProviderTests : AzureAdAuthInfoTests
    {
        [Test]
        public async Task GetAzureAdAccessTokenAsync_ReturnsAccessToken()
        {
            var expectedAccessToken = "accessToken";
            var mockTokenProvider = new Mock<IAzureAdTokenProvider>();

            mockTokenProvider
            .Setup(provider => provider.GetAzureAdAccessTokenAsync(It.IsAny<AzureAdAuthInfo>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedAccessToken);

            // Act
            string actualAccessToken = await mockTokenProvider.Object.GetAzureAdAccessTokenAsync(_azureAdAuthInfo);

            // Assert
            Assert.AreEqual(expectedAccessToken, actualAccessToken);
        }
    }
}
