using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Contrib.HttpClient;
using SoundForest.Clients.Spotify.Authentication.Infrastructure;
using SoundForest.Clients.Spotify.Authentication.Infrastructure.Options;
using SoundForest.Clients.Spotify.Authentication.Infrastructure.Responses;
using SoundForest.UnitTests.Common;
using System.Text.Json;

namespace SoundForest.Clients.Spotify.UnitTests.Authentication.Clients;
public class SpotifyAuthClientTests
{
    [Fact]
    internal void ConstructorGuards()
    {
        var assertion = new GuardClauseAssertion(AutoMoqDataAttribute.AutoMoqFixture);
        assertion.Verify(typeof(SpotifyAuthClient).GetConstructors());
    }

    [Theory]
    [AutoMoqData]
    internal async Task SpotifyAuthClient_AccessTokenAsync_Succeeds(
        ILogger<SpotifyAuthClient> logger,
        [Frozen] Mock<IOptions<SpotifyAuthOptions>> options,
        [Frozen] Mock<IOptions<JsonSerializerOptions>> serializer,
        SpotifyAuthOptions optionsValue,
        Mock<HttpMessageHandler> handler,
        Uri baseAddress,
        TokenResponse dto)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(optionsValue);
        serializer.Setup(opt => opt.Value).Returns(DefaultJsonSerializerOptions.Options);

        handler.SetupAnyRequest()
            .ReturnsJsonResponse(dto);

        var client = handler.CreateClient();
        client.BaseAddress = baseAddress;

        var sut = new SpotifyAuthClient(
                logger,
                options.Object,
                client,
                serializer.Object
            );

        // Act
        var result = await sut.AccessTokenAsync();

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result!.AccessToken.Should().Be(dto.access_token);
            result!.ExpiresIn.Should().Be(dto.expires_in);
            result!.TokenType.Should().Be(dto.token_type);
        }
    }

    [Theory]
    [AutoMoqData]
    internal async Task SpotifyAuthClient_AccessTokenAsync_Fails(
        ILogger<SpotifyAuthClient> logger,
        [Frozen] Mock<IOptions<SpotifyAuthOptions>> options,
        [Frozen] Mock<IOptions<JsonSerializerOptions>> serializer,
        SpotifyAuthOptions optionsValue,
        Mock<HttpMessageHandler> handler,
        Uri baseAddress)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(optionsValue);
        serializer.Setup(opt => opt.Value).Returns(DefaultJsonSerializerOptions.Options);

        handler.SetupAnyRequest()
            .Throws<Exception>();

        var client = handler.CreateClient();
        client.BaseAddress = baseAddress;

        var sut = new SpotifyAuthClient(
                logger,
                options.Object,
                client,
                serializer.Object
            );

        // Act
        var result = await sut.AccessTokenAsync();

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task SpotifyAuthClient_RefreshTokenAsync_Succeeds(
    ILogger<SpotifyAuthClient> logger,
    [Frozen] Mock<IOptions<SpotifyAuthOptions>> options,
    [Frozen] Mock<IOptions<JsonSerializerOptions>> serializer,
    SpotifyAuthOptions optionsValue,
    Mock<HttpMessageHandler> handler,
    Uri baseAddress,
    TokenResponse dto,
    string token)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(optionsValue);
        serializer.Setup(opt => opt.Value).Returns(DefaultJsonSerializerOptions.Options);

        handler.SetupAnyRequest()
            .ReturnsJsonResponse(dto);

        var client = handler.CreateClient();
        client.BaseAddress = baseAddress;

        var sut = new SpotifyAuthClient(
                logger,
                options.Object,
                client,
                serializer.Object
            );

        // Act
        var result = await sut.RefreshTokenAsync(token);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result!.AccessToken.Should().Be(dto.access_token);
            result!.ExpiresIn.Should().Be(dto.expires_in);
            result!.TokenType.Should().Be(dto.token_type);
        }
    }

    [Theory]
    [AutoMoqData]
    internal async Task SpotifyAuthClient_RefreshTokenAsync_Fails(
        ILogger<SpotifyAuthClient> logger,
        [Frozen] Mock<IOptions<SpotifyAuthOptions>> options,
        [Frozen] Mock<IOptions<JsonSerializerOptions>> serializer,
        SpotifyAuthOptions optionsValue,
        Mock<HttpMessageHandler> handler,
        Uri baseAddress,
        string token)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(optionsValue);
        serializer.Setup(opt => opt.Value).Returns(DefaultJsonSerializerOptions.Options);

        handler.SetupAnyRequest()
            .Throws<Exception>();

        var client = handler.CreateClient();
        client.BaseAddress = baseAddress;

        var sut = new SpotifyAuthClient(
                logger,
                options.Object,
                client,
                serializer.Object
            );

        // Act
        var result = await sut.RefreshTokenAsync(token);

        // Assert
        result.Should().BeNull();
    }
}
