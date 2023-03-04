using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using SoundForest.Clients.Auth0.Authentication.Application;
using SoundForest.Clients.Auth0.Authentication.Domain;
using SoundForest.Clients.Spotify.Authentication.Application;
using SoundForest.Exports.Processing.Infrastructure.Exporters.Services;
using SoundForest.UnitTests.Common;
using Xunit;
using AToken = SoundForest.Clients.Auth0.Authentication.Domain.Token;
using SToken = SoundForest.Clients.Spotify.Authentication.Domain.Token;

namespace SoundForest.Exports.UnitTests.Processing.Exporters;
public sealed class TokenServiceTests
{
    [Theory]
    [AutoMoqData]
    internal async Task TokenService_HappyFlow(
            ILogger<TokenService> logger,
            [Frozen] Mock<ISpotifyAuthClient> spotifyClient,
            [Frozen] Mock<IAuth0Client> auth0Client,
            [Frozen] Mock<IMemoryCache> cache,
            SToken stoken,
            AToken atoken,
            string username,
            User user
        )
    {
        // Arrange
        spotifyClient
           .Setup(c => c.AccessTokenAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(stoken);

        spotifyClient
            .Setup(c => c.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(stoken);

        auth0Client
            .Setup(c => c.M2mTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(atoken);

        auth0Client
            .Setup(c => c.UserTokenAsync(It.IsAny<string>(), username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var sut = new TokenService(logger, spotifyClient.Object, auth0Client.Object, cache.Object);

        // Act
        var result = await sut.GetM2mAccesToken(It.IsAny<CancellationToken>());

        // Assert
        result.Should().NotBeNullOrWhiteSpace();
    }
}
